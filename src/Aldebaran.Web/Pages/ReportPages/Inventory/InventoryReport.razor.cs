using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Pages.PurchaseOrderPages;
using Aldebaran.Web.Pages.ReportPages.Inventory.Components;
using Aldebaran.Web.Pages.ReportPages.Inventory.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Inventory
{
    public partial class InventoryReport
    {
        #region Injections
        [Inject]
        protected ILogger<AddPurchaseOrder> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }

        [Inject]
        protected IPurchaseOrderService PurchaseOrderService { get; set; }

        [Inject]
        protected IPurchaseOrderActivityService PurchaseOrderActivityService { get; set; }

        #endregion


        #region Variables

        protected InventoryFilter Filter;
        protected InventontoryViewModel ViewModel;
        private bool IsBusy = false;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new InventontoryViewModel
            {
                Lines = new List<InventoryLine>()
            };

            var references = await ItemReferenceService.GetInventoryReportReferences();

            ViewModel.Lines = (await GetLines(references)).ToList();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InventoryReportFilter>("Filtrar reporte de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InventoryFilter)result;
            //Todo: Aplicar filtro de refenrecias al ViewModel
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;
                //Todo: Remover filtro de refenrecias al ViewModel
                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Download(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Inventario.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report
        protected async Task<IEnumerable<InventoryLine>> GetLines(IEnumerable<ItemReference> references, CancellationToken ct = default)
        {
            var lines = new List<InventoryLine>();

            foreach (var line in references.Select(s => s.Item.Line).DistinctBy(d => d.LineId))
            {
                lines.Add(new InventoryLine { LineName = line.LineName, Items = (await GetItemsPerLine(references, line.LineId, ct)).ToList() });
            }

            return lines;
        }

        protected async Task<IEnumerable<InventoryItem>> GetItemsPerLine(IEnumerable<ItemReference> references, short lineId, CancellationToken ct = default)
        {
            var items = new List<InventoryItem>();

            foreach (var item in references.Select(s => s.Item).Where(w => w.LineId == lineId && w.IsActive && w.IsExternalInventory).DistinctBy(d => d.ItemId))
            {
                items.Add(new InventoryItem { InternalReference = item.InternalReference, ItemName = item.ItemName, InventoryReferences = (await GetReferencesPerItem(references, item.ItemId, ct)).ToList() });
            }

            return items;
        }

        protected async Task<IEnumerable<InventoryReference>> GetReferencesPerItem(IEnumerable<ItemReference> references, int itemId, CancellationToken ct = default)
        {
            var inventoryReferences = new List<InventoryReference>();

            foreach (var reference in references.Where(w => w.ItemId == itemId && w.IsActive))
            {
                inventoryReferences.Add(new InventoryReference
                {
                    ReferenceName = reference.ReferenceName,
                    AvailableAmount = reference.InventoryQuantity - reference.ReservedQuantity - reference.OrderedQuantity,
                    FreeZone = await GetFreeZoneReference(reference.ReferenceId, ct),
                    PurchaseOrders = (await GetPurchaseOrderPerReference(reference.ReferenceId, ct)).ToList()
                });
            }

            return inventoryReferences;
        }

        protected async Task<int> GetFreeZoneReference(int referenceId, CancellationToken ct = default)
        {
            var freeZoneWarehouse = await WarehouseService.FindByCodeAsync(2, ct);

            var referenceWarehouse = await ReferencesWarehouseService.GetByReferenceAndWarehouseIdAsync(referenceId, freeZoneWarehouse.WarehouseId, ct);

            return referenceWarehouse.Quantity;
        }

        protected async Task<IEnumerable<InventoryPurchaseOrder>> GetPurchaseOrderPerReference(int referenceId, CancellationToken ct = default)
        {
            var inventoryPurchaseOrders = new List<InventoryPurchaseOrder>();

            var warehouses = await WarehouseService.GetAsync();

            var purchaseOrdersActivity = await PurchaseOrderService.GetTransitByReferenceId(referenceId, ct);

            foreach (var purchaseOrder in purchaseOrdersActivity)
            {
                var details = purchaseOrder.PurchaseOrderDetails;
                var activities = purchaseOrder.PurchaseOrderActivities;

                var date = purchaseOrder.ExpectedReceiptDate;

                foreach (var detail in details.Where(w => w.ReferenceId == referenceId))
                {
                    var warehouse = warehouses.FirstOrDefault(f => f.WarehouseId == detail.WarehouseId);

                    var inventoryPurchaseOrder = inventoryPurchaseOrders.FirstOrDefault(f => f.Date == date && f.Warehouse == warehouse.WarehouseName);

                    if (inventoryPurchaseOrder != null)
                    {
                        inventoryPurchaseOrder.Total += detail.RequestedQuantity;
                        continue;
                    }

                    inventoryPurchaseOrder = new InventoryPurchaseOrder
                    {
                        Date = date,
                        Total = detail.RequestedQuantity,
                        Warehouse = warehouse.WarehouseName,
                        Activity = new List<InventoryActivity>()
                    };
                }
            }
            return inventoryPurchaseOrders;
        }


        #endregion
    }
}
