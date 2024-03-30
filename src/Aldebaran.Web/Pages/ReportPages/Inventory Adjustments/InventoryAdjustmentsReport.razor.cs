using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.Components;
using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments
{
    public partial class InventoryAdjustmentsReport
    {
        #region Injections
        [Inject]
        protected ILogger<InventoryAdjustmentsReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IInventoryAdjustmentReportService InventoryAdjustmentReportService { get; set; }
        #endregion

        #region Variables
        protected InventoryAdjustmentsFilter Filter;
        protected InventoryAdjustmentsViewModel ViewModel;
        private bool IsBusy = false;
        private readonly bool IsLoadingData = false;
        private IEnumerable<InventoryAdjustmentReport> DataReport { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await RedrawReportAsync();
        }
        #endregion

        #region Fill Data Report

        async Task<List<InventoryAdjustmentsViewModel.Adjustment>> GetAdjustmentsAsync(CancellationToken ct = default)
        {
            var adjustments = new List<InventoryAdjustmentsViewModel.Adjustment>();
            var uniqueAdjustments = DataReport.Select(s => new
            {
                s.AdjustmentDate,
                s.AdjustmentId,
                s.AdjustmentReason,
                s.AdjustmentType,
                s.CreationDate,
                s.Employee,
                s.Notes
            }).DistinctBy(d => d.AdjustmentId);

            foreach (var adjustment in uniqueAdjustments)
            {
                var warehouses = await GetAdjustmentsAsync(adjustment.AdjustmentId, ct);
                adjustments.Add(new InventoryAdjustmentsViewModel.Adjustment
                {
                    AdjustmentDate = adjustment.AdjustmentDate,
                    AdjustmentId = adjustment.AdjustmentId,
                    AdjustmentReason = adjustment.AdjustmentReason,
                    AdjustmentType = adjustment.AdjustmentType,
                    CreationDate = adjustment.CreationDate,
                    Employee = adjustment.Employee,
                    Notes = adjustment.Notes,
                    Warehouses = warehouses
                });
            }

            return adjustments;
        }

        async Task<List<InventoryAdjustmentsViewModel.Warehouse>> GetAdjustmentsAsync(int adjustmentId, CancellationToken ct = default)
        {
            var warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>();
            var uniqueWarehouses = DataReport.Where(w => w.AdjustmentId == adjustmentId).Select(s => new { s.WarehouseId, s.WarehouseName }).DistinctBy(d => d.WarehouseId);
            foreach (var warehouse in uniqueWarehouses)
            {
                var lines = await GetWarehouseLines(adjustmentId, warehouse.WarehouseId, ct);
                warehouses.Add(new InventoryAdjustmentsViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = lines
                });
            }
            return warehouses;
        }

        async Task<List<InventoryAdjustmentsViewModel.Line>> GetWarehouseLines(int adjustmentId, int warehouseId, CancellationToken ct = default)
        {
            var reportLines = new List<InventoryAdjustmentsViewModel.Line>();
            var uniqueLines = DataReport.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId).Select(s => new { s.LineId, s.LineCode, s.LineName }).DistinctBy(d => d.LineId).OrderBy(o => o.LineName);
            foreach (var line in uniqueLines)
            {
                var items = await GetLineItems(adjustmentId, warehouseId, line.LineId, ct);
                reportLines.Add(new InventoryAdjustmentsViewModel.Line
                {
                    LineCode = line.LineCode,
                    LineName = line.LineName,
                    Items = items
                });
            }
            return reportLines.DistinctBy(d => d.LineCode).ToList();
        }

        async Task<List<InventoryAdjustmentsViewModel.Item>> GetLineItems(int adjustmentId, int warehouseId, short lineId, CancellationToken ct = default)
        {
            var reportItems = new List<InventoryAdjustmentsViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId && w.LineId == lineId).Select(s => new { s.ItemId, s.ItemName, s.InternalReference })
                                    .DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName))
            {
                var references = await GetItemReferences(adjustmentId, warehouseId, item.ItemId, ct);
                reportItems.Add(new InventoryAdjustmentsViewModel.Item
                {
                    ItemName = item.ItemName,
                    InternalReference = item.InternalReference,
                    References = references
                });
            }

            return reportItems.DistinctBy(d => d.ItemName).ToList();
        }

        async Task<List<InventoryAdjustmentsViewModel.Reference>> GetItemReferences(int adjustmentId, int warehouseId, int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<InventoryAdjustmentsViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId && w.ItemId == itemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.ReferenceCode, s.AvailableAmount })
                                        .DistinctBy(d => d.ReferenceName).OrderBy(o => o.ReferenceName))
            {
                reportReferences.Add(new InventoryAdjustmentsViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    ReferenceCode = reference.ReferenceCode,
                    AvailableAmount = reference.AvailableAmount
                });
            }

            return reportReferences;
        }

        #endregion

        #region Events

        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            DataReport = await InventoryAdjustmentReportService.GetInventoryAdjustmentReportDataAsync(filter, ct);

            ViewModel = new InventoryAdjustmentsViewModel()
            {
                Adjustments = await GetAdjustmentsAsync(ct)
            };
        }

        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InventoryAdjustmentsReportFilter>("Filtrar reporte de ajustes de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InventoryAdjustmentsFilter)result;

            await RedrawReportAsync(await SetReportFiterAsync(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }

        async Task<string> SetReportFiterAsync(InventoryAdjustmentsFilter filter, CancellationToken ct = default)
        {
            var filters = new List<string>();

            if (filter.CreationDate?.StartDate != null)
                filters.Add($"@CreationDateFrom = '{filter.CreationDate.StartDate.Value:yyyyMMdd}'");

            if (filter.CreationDate?.EndDate != null)
                filters.Add($"@CreationDateTo = '{filter.CreationDate.EndDate.Value:yyyyMMdd}'");

            if (filter.AdjustmentDate?.StartDate != null)
                filters.Add($"@AdjustmentDateFrom = '{filter.AdjustmentDate.StartDate.Value:yyyyMMdd}'");

            if (filter.AdjustmentId.HasValue)
                filters.Add($"@AdjustmentId = {filter.AdjustmentId}");

            if (filter.EmployeeId.HasValue)
                filters.Add($"@EmployeeId = {filter.EmployeeId}");

            if (filter.ItemReferences.Count > 0)
                filters.Add($"@ReferenceIds = '{string.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'");

            if (filter.AdjustmentReasonId.HasValue)
                filters.Add($"@AdjustmentReasonId = {filter.AdjustmentReasonId}");

            if (filter.AdjustmentTypeId.HasValue)
                filters.Add($"@AdjustmentTypeId = {filter.AdjustmentTypeId}");

            return string.Join(", ", filters);
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
        async Task Download()
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-adjustments-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ajustes de inventario.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
