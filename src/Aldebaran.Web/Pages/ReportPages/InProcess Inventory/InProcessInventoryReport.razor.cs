using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.Components;
using Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.ViewModel;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.InProcess_Inventory
{
    public partial class InProcessInventoryReport
    {
        #region Injections
        [Inject]
        protected ILogger<InProcessInventoryReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }

        #endregion

        #region Variables
        protected InProcessInventoryFilter Filter;
        protected InProcessInventoryViewModel ViewModel;
        List<InProcessInventoryViewModel.Warehouse> UniqueWarehouses = new List<InProcessInventoryViewModel.Warehouse>();
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new InProcessInventoryViewModel
            {
                Lines = new List<InProcessInventoryViewModel.Line>()
            };

            ViewModel.Lines = (await GetReporLinesAsync()).ToList();


            UniqueWarehouses = ViewModel.Lines.SelectMany(s => s.Items)
                .SelectMany(item => item.References.SelectMany(reference => reference.Warehouses))
                .DistinctBy(w => w.WarehouseId)
                .ToList();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InProcessInventoryReportFilter>("Filtrar reporte de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InProcessInventoryFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inprocess-inventory-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Inventario en proceso.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }

        #endregion

        #region Fill Data Report

        protected async Task<IEnumerable<InProcessInventoryViewModel.Line>> GetReporLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<InProcessInventoryViewModel.Line>();

            var references = await ItemReferenceService.GetReportsReferences(isExternalInventory:true,ct:ct);

            foreach (var line in references.Select(s => s.Item.Line).DistinctBy(l => l.LineId).OrderBy(o=>o.LineName))
            {
                lines.Add(new InProcessInventoryViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(references, line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<InProcessInventoryViewModel.Item>> GetReportItemsByLineIdAsync(IEnumerable<ItemReference> references, short lineId, CancellationToken ct = default)
        {
            var items = new List<InProcessInventoryViewModel.Item>();

            foreach (var item in references.Where(w => w.Item.LineId == lineId).Select(s => s.Item).DistinctBy(l => l.ItemId).OrderBy(o=>o.ItemName))
            {
                items.Add(new InProcessInventoryViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(references, item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<InProcessInventoryViewModel.Reference>> GetReferencesByItemIdAsync(IEnumerable<ItemReference> references, int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<InProcessInventoryViewModel.Reference>();

            foreach (var reference in references.Where(w => w.ItemId == itemId).OrderBy(o=>o.ReferenceCode))
            {
                reportReferences.Add(new InProcessInventoryViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    InProcessAmount = reference.WorkInProcessQuantity,
                    Warehouses = (await GetWarehousesByReferenceIdAsync(reference.ReferenceId, ct)).ToList()

                }); ;
            }

            return reportReferences;
        }

        protected async Task<IEnumerable<InProcessInventoryViewModel.Warehouse>> GetWarehousesByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        { 
            var warehouses = new List<InProcessInventoryViewModel.Warehouse>();

            var referenceWarehouses = await ReferencesWarehouseService.GetByReferenceIdAsync(referenceId, ct);

            foreach (var warehouse in referenceWarehouses.OrderBy(o=>o.Warehouse.WarehouseName))
            {
                warehouses.Add(new InProcessInventoryViewModel.Warehouse 
                {
                    WarehouseId = warehouse.WarehouseId,
                    Amount = warehouse.Quantity,
                    WarehouseName = warehouse.Warehouse.WarehouseName,
                });

            }

            return warehouses;
        
        }

        #endregion

        
    }
}
