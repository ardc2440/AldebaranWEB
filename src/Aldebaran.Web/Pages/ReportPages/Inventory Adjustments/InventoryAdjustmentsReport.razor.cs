using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
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

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            var dataReport = await InventoryAdjustmentReportService.GetInventoryAdjustmentReportDataAsync();

            ViewModel = new InventoryAdjustmentsViewModel()
            {
                Adjustments = (await GetAdjustmentsAsync(dataReport.ToList())).ToList()
            };
        }
        #endregion

        #region Fill Data Report

        async static Task<List<InventoryAdjustmentsViewModel.Adjustment>> GetAdjustmentsAsync(List<InventoryAdjustmentReport> data, CancellationToken ct = default)
        {
            var adjustments = new List<InventoryAdjustmentsViewModel.Adjustment>();
            var uniqueAdjustments = data.Select(s => new
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
                var warehouses = await GetAdjustmentsAsync(data, adjustment.AdjustmentId, ct);
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

        async static Task<List<InventoryAdjustmentsViewModel.Warehouse>> GetAdjustmentsAsync(List<InventoryAdjustmentReport> data, int adjustmentId, CancellationToken ct = default)
        {
            var warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>();
            var uniqueWarehouses = data.Where(w => w.AdjustmentId == adjustmentId).Select(s => new { s.WarehouseId, s.WarehouseName }).DistinctBy(d => d.WarehouseId);
            foreach (var warehouse in uniqueWarehouses)
            {
                var lines = await GetWarehouseLines(data, adjustmentId, warehouse.WarehouseId, ct);
                warehouses.Add(new InventoryAdjustmentsViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = lines
                });
            }
            return warehouses;
        }

        async static Task<List<InventoryAdjustmentsViewModel.Line>> GetWarehouseLines(List<InventoryAdjustmentReport> data, int adjustmentId, int warehouseId, CancellationToken ct = default)
        {
            var reportLines = new List<InventoryAdjustmentsViewModel.Line>();
            var uniqueLines = data.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId).Select(s => new { s.LineCode, s.LineName }).DistinctBy(d => d.LineCode);
            foreach (var line in uniqueLines)
            {
                var items = await GetLineItems(data, adjustmentId, warehouseId, line.LineCode, ct);
                reportLines.Add(new InventoryAdjustmentsViewModel.Line
                {
                    LineCode = line.LineCode,
                    LineName = line.LineName,
                    Items = items
                });
            }
            return reportLines.DistinctBy(d => d.LineCode).ToList();
        }

        async static Task<List<InventoryAdjustmentsViewModel.Item>> GetLineItems(List<InventoryAdjustmentReport> data, int adjustmentId, int warehouseId, string lineCode, CancellationToken ct = default)
        {
            var reportItems = new List<InventoryAdjustmentsViewModel.Item>();
            var uniqueItems = data.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId && w.LineCode.Equals(lineCode)).Select(s => new { s.ItemName, s.InternalReference }).DistinctBy(d => new { d.ItemName, d.InternalReference });
            foreach (var item in uniqueItems)
            {
                var references = await GetItemReferences(data, adjustmentId, warehouseId, item.ItemName, ct);
                reportItems.Add(new InventoryAdjustmentsViewModel.Item
                {
                    ItemName = item.ItemName,
                    InternalReference = item.InternalReference,
                    References = references
                });
            }
            return reportItems.DistinctBy(d => d.ItemName).ToList();
        }

        async static Task<List<InventoryAdjustmentsViewModel.Reference>> GetItemReferences(List<InventoryAdjustmentReport> data, int adjustmentId, int warehouseId, string itemName, CancellationToken ct = default)
        {
            var reportReferences = new List<InventoryAdjustmentsViewModel.Reference>();
            foreach (var reference in data.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId && w.ItemName.Equals(itemName)).Select(s => new { s.ReferenceName, s.ReferenceCode, s.AvailableAmount }).DistinctBy(d => new { d.ReferenceCode }))
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
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InventoryAdjustmentsReportFilter>("Filtrar reporte de ajustes de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InventoryAdjustmentsFilter)result;
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
