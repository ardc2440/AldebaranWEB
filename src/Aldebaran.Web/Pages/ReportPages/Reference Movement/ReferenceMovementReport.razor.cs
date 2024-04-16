using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.Components;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement
{
    public partial class ReferenceMovementReport
    {
        #region Injections
        [Inject]
        protected ILogger<ReferenceMovementReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IReferenceMovementReportService ReferenceMovementReportService { get; set; }

        #endregion

        #region Variables
        protected ReferenceMovementFilter Filter;
        protected ReferenceMovementViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.ReferenceMovementReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await RedrawReportAsync();
        }
        #endregion

        #region Events

        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                DataReport = await ReferenceMovementReportService.GetReferenceMovementReportDataAsync(filter, ct);

                ViewModel = new ReferenceMovementViewModel
                {
                    Lines = (await GetReporLinesAsync()).ToList()
                };
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReportFilter>("Filtrar reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ReferenceMovementFilter)result;

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
                Filter = null;

                await RedrawReportAsync();

                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-movement-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Movimientos de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "inventory-movement-report-container");
            }
            IsBusy = false;
        }

        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion

        #region Fill Data Report

        protected async Task<IEnumerable<ReferenceMovementViewModel.Line>> GetReporLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<ReferenceMovementViewModel.Line>();

            foreach (var line in DataReport.Select(s => new { s.LineId, s.LineName, s.LineCode })
                                    .DistinctBy(d => d.LineId).OrderBy(o => o.LineName))
            {
                lines.Add(new ReferenceMovementViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Item>> GetReportItemsByLineIdAsync(short lineId, CancellationToken ct = default)
        {
            var items = new List<ReferenceMovementViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.LineId == lineId).Select(s => new { s.ItemId, s.InternalReference, s.ItemName })
                                    .DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName))
            {
                items.Add(new ReferenceMovementViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Reference>> GetReferencesByItemIdAsync(int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<ReferenceMovementViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.ItemId == itemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.RequestedQuantity, s.ReservedQuantity, s.ReferenceCode })
                                        .DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName))
            {
                reportReferences.Add(new ReferenceMovementViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    RequestedQuantity = reference.RequestedQuantity,
                    ReservedQuantity = reference.ReservedQuantity,
                    ReferenceCode = reference.ReferenceCode,
                    Warehouses = (await GetWarehousesByReferenceIdAsync(reference.ReferenceId, ct)).ToList()

                }); ;
            }

            return reportReferences;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Warehouse>> GetWarehousesByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var warehouses = new List<ReferenceMovementViewModel.Warehouse>();

            foreach (var warehouse in DataReport.Where(w => w.ReferenceId == referenceId).Select(s => new { s.WarehouseId, s.Amount, s.WarehouseName })
                                        .DistinctBy(d => d.WarehouseId).OrderBy(o => o.WarehouseName))
            {
                warehouses.Add(new ReferenceMovementViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    Amount = warehouse.Amount,
                    WarehouseName = warehouse.WarehouseName,
                });

            }

            return warehouses;

        }

        #endregion

    }
}
