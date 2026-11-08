using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.Components;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using ReportFilter = Aldebaran.Application.Services.Models.Reports.WarehouseTransferReportFilter;

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
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<WarehouseTransfersReportFilter>("Filtrar reporte de traslados entre bodegas", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (WarehouseTransfersFilter)result;

            await RedrawReport(ToReportFilter(Filter));

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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "warehouse-transfer-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Traslados entre bodegas.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "warehouse-transfer-report-container");
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
        async Task RedrawReport(ReportFilter filter, CancellationToken ct = default)
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
                StateHasChanged();
            }

        }
        /// <summary>Filtros del caso de uso (los valores viajan como parámetros del SP, nunca concatenados en el SQL).</summary>
        static ReportFilter ToReportFilter(WarehouseTransfersFilter filter) => new()
        {
                TargetWarehouseId = filter.TargetWarehouseId,
                SourceWarehouseId = filter.SourceWarehouseId,
                AdjustmentDateFrom = filter.AdjustmentDate?.StartDate,
                AdjustmentDateTo = filter.AdjustmentDate?.EndDate,
                NationalizationNumber = filter.NationalizationNumber,
                StatusId = filter.StatusDocumentTypeId,
                ReferenceIds = filter.ItemReferences?.Any() == true ? string.Join(",", filter.ItemReferences.Select(s => s.ReferenceId)) : null
        };
        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
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
