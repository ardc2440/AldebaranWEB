using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.Components;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Stock
{
    public partial class WarehouseStockReport
    {
        #region Injections
        [Inject]
        protected ILogger<WarehouseStockReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IWarehouseStockReportService WarehouseStockReportService { get; set; }
        #endregion

        #region Variables
        protected WarehouseStockFilter Filter;
        protected WarehouseStockViewModel ViewModel;
        private bool IsBusy = false;

        private IEnumerable<Application.Services.Models.Reports.WarehouseStockReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            DataReport = await WarehouseStockReportService.GetWarehouseStockReportDataAsync();

            ViewModel = new WarehouseStockViewModel()
            {
                Warehouses = (await GetReportWarehousesAsync()).ToList()
            };

        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<WarehouseStockReportFilter>("Filtrar reporte de existencias de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (WarehouseStockFilter)result;
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
        async Task Download()
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "warehouse-stock-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Existencias de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report

        protected async Task<IEnumerable<WarehouseStockViewModel.Warehouse>> GetReportWarehousesAsync(CancellationToken ct = default)
        {
            var warehousesList = new List<WarehouseStockViewModel.Warehouse>();

            foreach (var warehouse in DataReport.Select(s => new { s.WarehouseId, s.WarehouseName })
                                        .DistinctBy(d => d.WarehouseId).OrderBy(o => o.WarehouseName))
            {
                warehousesList.Add(new WarehouseStockViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = (await GetReporLinesAsync(warehouse.WarehouseId, ct)).ToList()
                });
            }

            return warehousesList;
        }

        protected async Task<IEnumerable<WarehouseStockViewModel.Line>> GetReporLinesAsync(short warehouseId, CancellationToken ct = default)
        {
            var lines = new List<WarehouseStockViewModel.Line>();

            foreach (var line in DataReport.Where(w => w.WarehouseId == warehouseId).Select(s => new { s.LineId, s.LineCode, s.LineName })
                                    .DistinctBy(d => d.LineId).OrderBy(o => o.LineName))
            {
                lines.Add(new WarehouseStockViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(warehouseId, line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<WarehouseStockViewModel.Item>> GetReportItemsByLineIdAsync(short warehouseId, short lineId, CancellationToken ct = default)
        {
            var items = new List<WarehouseStockViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.LineId == lineId && w.WarehouseId == warehouseId).Select(s => new { s.ItemId, s.InternalReference, s.ItemName })
                                    .DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName))
            {
                items.Add(new WarehouseStockViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(warehouseId, item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<WarehouseStockViewModel.Reference>> GetReferencesByItemIdAsync(short warehouseId, int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<WarehouseStockViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.ItemId == itemId && w.WarehouseId == warehouseId).Select(s => new { s.ReferenceId, s.ReferenceName, s.AvailableAmount, s.ProviderReferenceName, s.ReferenceCode })
                                        .DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName))
            {
                reportReferences.Add(new WarehouseStockViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    AvailableAmount = reference.AvailableAmount,
                    ProviderReferenceName = reference.ProviderReferenceName,
                    ReferenceCode = reference.ReferenceCode
                }); ;
            }

            return reportReferences;
        }

        #endregion

    }
}
