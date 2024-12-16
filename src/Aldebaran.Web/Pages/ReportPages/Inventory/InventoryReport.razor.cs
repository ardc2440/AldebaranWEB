using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Inventory.Components;
using Aldebaran.Web.Pages.ReportPages.Inventory.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Inventory
{
    public partial class InventoryReport
    {
        #region Injections
        [Inject]
        protected ILogger<InventoryReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IInventoryReportService InventoryReportService { get; set; }

        #endregion

        #region Variables

        protected InventoryFilter Filter;
        protected InventoryViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private ElementReference h3ElementRef;

        private IEnumerable<Application.Services.Models.Reports.InventoryReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Reset();
            }
        }
        #endregion

        #region Events
        async Task Reset()
        {
            Filter = null;
            ViewModel = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                DataReport = await InventoryReportService.GetInventoryReportDataAsync(filter, ct);

                ViewModel = new InventoryViewModel
                {
                    Lines = (await GetLinesAsync(ct)).ToList()
                };
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InventoryReportFilter>("Filtrar reporte de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;

            Filter = (InventoryFilter)result;

            var referenceIdsFilter = "";

            if (Filter.ItemReferences.Count > 0)
                referenceIdsFilter = String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId));

            await RedrawReportAsync(referenceIdsFilter);

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                await Reset();
            }
        }

        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Inventario.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "inventory-report-container");
            }
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        async Task ToggleReadMorePage()
        {
            await JSRuntime.InvokeVoidAsync("readMoreTogglePage", "toggleLinkPage");
        }
        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
                
        #endregion

        #region Fill Data Report
        protected async Task<IEnumerable<InventoryViewModel.Line>> GetLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<InventoryViewModel.Line>();

            foreach (var line in DataReport.Select(s => new { s.LineId, s.LineName }).DistinctBy(d => d.LineId).OrderBy(o => o.LineName))
            {
                lines.Add(new InventoryViewModel.Line { LineName = line.LineName, Items = (await GetItemsPerLineAsync(line.LineId, ct)).ToList() });
            }

            return lines;
        }

        protected async Task<IEnumerable<InventoryViewModel.Item>> GetItemsPerLineAsync(short lineId, CancellationToken ct = default)
        {
            var items = new List<InventoryViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.LineId == lineId).Select(s => new { s.ItemId, s.ItemName, s.InternalReference }).DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName))
            {
                items.Add(new InventoryViewModel.Item { InternalReference = item.InternalReference, ItemName = item.ItemName, References = (await GetReferencesPerItemAsync(item.ItemId, ct)).ToList() });
            }

            return items;
        }

        protected async Task<IEnumerable<InventoryViewModel.Reference>> GetReferencesPerItemAsync(int itemId, CancellationToken ct = default)
        {
            var inventoryReferences = new List<InventoryViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.ItemId == itemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.AvailableAmount, s.FreeZone, s.LocalWarehouse }).DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName))
            //references.Where(w => w.ItemId == itemId && w.IsActive).OrderBy(o=>o.ReferenceCode))
            {
                inventoryReferences.Add(new InventoryViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    AvailableAmount = reference.AvailableAmount,
                    LocalWarehouse = reference.LocalWarehouse,
                    FreeZone = reference.FreeZone,
                    PurchaseOrders = (await GetPurchaseOrderPerReferenceAsync(reference.ReferenceId, ct)).ToList()
                });
            }

            return inventoryReferences;
        }

        protected async Task<IEnumerable<InventoryViewModel.PurchaseOrder>> GetPurchaseOrderPerReferenceAsync(int referenceId, CancellationToken ct = default)
        {
            var inventoryPurchaseOrders = new List<InventoryViewModel.PurchaseOrder>();

            foreach (var purchaseOrder in DataReport.Where(w => w.ReferenceId == referenceId && w.PurchaseOrderId > 0)
                                            .Select(s => new { s.PurchaseOrderId, s.OrderDate, s.Warehouse, s.Total })
                                            .DistinctBy(d => new { d.PurchaseOrderId, d.Warehouse })
                                            .OrderBy(o => o.OrderDate))
            {
                inventoryPurchaseOrders.Add(new InventoryViewModel.PurchaseOrder
                {
                    Date = purchaseOrder.OrderDate,
                    Total = purchaseOrder.Total ?? 0,
                    Warehouse = purchaseOrder.Warehouse,
                    Activities = await GetOrderActivitiesAsync(purchaseOrder.PurchaseOrderId, referenceId, ct)
                });
            }

            return inventoryPurchaseOrders;
        }

        protected async Task<List<InventoryViewModel.Activity>> GetOrderActivitiesAsync(int purchaseOrderId, int referenceId, CancellationToken ct = default)
        {
            var activities = new List<InventoryViewModel.Activity>();

            foreach (var item in DataReport.Where(w => w.PurchaseOrderId == purchaseOrderId && w.ReferenceId == referenceId && w.Description != null && w.ActivityId != null)
                                        .Select(s => new { s.ActivityDate, s.Description, s.ActivityId })
                                        .DistinctBy(d => d.ActivityId)
                                        .Select(s => new InventoryViewModel.Activity
                                        {
                                            Date = s.ActivityDate,
                                            Description = s.Description,
                                        }))
                activities.Add(item);

            return activities;
        }
        #endregion
    }
}
