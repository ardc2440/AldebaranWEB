using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.Components;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers
{
    public partial class WarehouseTransfersReport
    {
        #region Injections
        [Inject]
        protected ILogger<WarehouseTransfersReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IWarehouseTransferReportService WarehouseTransferReportService { get; set; }
        #endregion

        #region Variables
        protected WarehouseTransfersFilter Filter;
        protected WarehouseTransfersViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;

        private IEnumerable<Application.Services.Models.Reports.WarehouseTransferReport> DataReport { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await RedrawReport();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<WarehouseTransfersReportFilter>("Filtrar reporte de traslados entre bodegas", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (WarehouseTransfersFilter)result;

            await RedrawReport(await SetReportFilter(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;

                await RedrawReport();

                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Download()
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "warehouse-transfer-report-container");
            var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Traslados entre bodegas.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        async Task RedrawReport(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                DataReport = await WarehouseTransferReportService.GetWarehouseTransferReportDataAsync(filter, ct);

                ViewModel = new WarehouseTransfersViewModel
                {
                    WarehouseTransfers = await GetWarehouseTransfers(ct)
                };
            }
            finally
            {
                IsLoadingData = false;
            }

        }
        async Task<string> SetReportFilter(WarehouseTransfersFilter filter, CancellationToken ct = default)
        {
            var filters = new List<string>();

            if (filter.TargetWarehouseId.HasValue)
                filters.Add($"@TargetWarehouseId = {filter.TargetWarehouseId}");

            if (filter.SourceWarehouseId.HasValue)
                filters.Add($"@SourceWarehouseId = {filter.SourceWarehouseId}");

            if (filter.ItemReferences?.Any() == true)
                filters.Add($"@ReferenceIds = '{string.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'");

            if (filter.AdjustmentDate?.StartDate != null)
                filters.Add($"@AdjustmentDateFrom = '{filter.AdjustmentDate.StartDate.Value:yyyyMMdd}'");

            if (filter.AdjustmentDate?.EndDate != null)
                filters.Add($"@AdjustmentDateTo = '{filter.AdjustmentDate.EndDate.Value:yyyyMMdd}'");

            if (!string.IsNullOrEmpty(filter.NationalizationNumber))
                filters.Add($"@NationalizationNumber = {filter.NationalizationNumber.Trim()}");

            if (filter.StatusDocumentTypeId.HasValue)
                filters.Add($"@StatusId = {filter.StatusDocumentTypeId}");

            return string.Join(", ", filters);
        }
        #endregion

        #region Fill Data Report
        async Task<List<WarehouseTransfersViewModel.WarehouseTransfer>> GetWarehouseTransfers(CancellationToken ct = default)
        {
            var warehouseTransfers = new List<WarehouseTransfersViewModel.WarehouseTransfer>();

            foreach (var warehouseTransfer in DataReport.Select(s => new { s.TransferId, s.Date, s.SourceWarehouseName, s.TargetWarehouseName, s.RegistrationDate, s.NationalizationNumber, s.TransferStatus })
                                                .DistinctBy(d => d.TransferId)
                                                .OrderBy(o => o.Date))
            {
                warehouseTransfers.Add(new WarehouseTransfersViewModel.WarehouseTransfer
                {
                    Date = warehouseTransfer.Date,
                    NationalizationNumber = warehouseTransfer.NationalizationNumber,
                    RegistrationDate = warehouseTransfer.RegistrationDate,
                    SourceWarehouseName = warehouseTransfer.SourceWarehouseName,
                    TargetWarehouseName = warehouseTransfer.TargetWarehouseName,
                    Status = warehouseTransfer.TransferStatus,
                    References = await GetWarehouseTransferReferences(warehouseTransfer.TransferId, ct)
                });
            }
            return warehouseTransfers;
        }
        async Task<List<WarehouseTransfersViewModel.Reference>> GetWarehouseTransferReferences(int transferId, CancellationToken ct = default)
        {
            var transferDetails = new List<WarehouseTransfersViewModel.Reference>();

            foreach (var transferDetail in DataReport.Where(w => w.TransferId == transferId).Select(s => new { s.ReferenceId, s.ItemReference, s.ItemName, s.ReferenceName, s.ReferenceCode, s.Amount })
                                                .DistinctBy(d => d.ReferenceId)
                                                .OrderBy(o => o.ItemName)
                                                .OrderBy(o => o.ReferenceName))
            {
                transferDetails.Add(new WarehouseTransfersViewModel.Reference
                {
                    ItemReference = transferDetail.ItemReference,
                    ReferenceName = transferDetail.ReferenceName,
                    ReferenceCode = transferDetail.ReferenceCode,
                    ItemName = transferDetail.ItemName,
                    Amount = transferDetail.Amount
                });
            }

            return transferDetails;
        }

        #endregion
    }
}
