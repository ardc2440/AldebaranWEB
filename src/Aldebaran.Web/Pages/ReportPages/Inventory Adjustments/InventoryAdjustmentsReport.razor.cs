using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.Components;
using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.JSInterop;
using Radzen;
using System.Collections.Generic;

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
        protected IAdjustmentService AdjustmentService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IAdjustmentDetailService AdjustmentDetailService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IInventoryAdjustmentReportService InventoryAdjustmentReportService { get; set; }
        #endregion

        #region Variables
        protected InventoryAdjustmentsFilter Filter;
        protected InventoryAdjustmentsViewModel ViewModel;
        protected IEnumerable<InventoryAdjustmentReport> dataReport;
        private bool IsBusy = false;


        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            dataReport = await InventoryAdjustmentReportService.GetInventoryAdjustmentReportDataAsync();

            ViewModel = new InventoryAdjustmentsViewModel()
            {
                Adjustments = (await GetAdjustmentsAsync()).ToList()
            };
        }
        #endregion

        #region Fill Data Report

        async Task<List<InventoryAdjustmentsViewModel.Adjustment>> GetAdjustmentsAsync(CancellationToken ct = default)
        {
            var adjustments = new List<InventoryAdjustmentsViewModel.Adjustment>();

            foreach (var adjustment in dataReport.Select(s => new { s.AdjustmentDate, s.AdjustmentId, s.AdjustmentReason, s.AdjustmentType, s.CreationDate, s.Employee, s.Notes }).DistinctBy(d => d.AdjustmentId))
            {
                adjustments.Add(new InventoryAdjustmentsViewModel.Adjustment
                {
                    AdjustmentDate = adjustment.AdjustmentDate,
                    AdjustmentId = adjustment.AdjustmentId,
                    AdjustmentReason = adjustment.AdjustmentReason,
                    AdjustmentType = adjustment.AdjustmentType,
                    CreationDate = adjustment.CreationDate,
                    Employee = adjustment.Employee,
                    Notes = adjustment.Notes,
                    Warehouses = await GetAdjustmentsAsync(adjustment.AdjustmentId, ct)
                });
            }

            return adjustments;
        }

        async Task<List<InventoryAdjustmentsViewModel.Warehouse>> GetAdjustmentsAsync(int adjustmentId, CancellationToken ct = default)
        {
            var warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>();

            foreach (var warehouse in dataReport.Where(w => w.AdjustmentId == adjustmentId).Select(s => new { s.WarehouseId, s.WarehouseName }).DistinctBy(d => d.WarehouseId))
            {
                warehouses.Add(new InventoryAdjustmentsViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = await GetWarehouseLines(adjustmentId, warehouse.WarehouseId, ct)
                });
            }

            return warehouses;
        }

        async Task<List<InventoryAdjustmentsViewModel.Line>> GetWarehouseLines(int adjustmentId, int warehouseId, CancellationToken ct = default)
        {
            var reportLines = new List<InventoryAdjustmentsViewModel.Line>();

            foreach (var line in dataReport.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId).Select(s => new { s.LineCode, s.LineName }).DistinctBy(d => d.LineCode))
            {
                reportLines.Add(new InventoryAdjustmentsViewModel.Line
                {
                    LineCode = line.LineCode,
                    LineName = line.LineName,
                    Items = await GetLineItems(adjustmentId, warehouseId, line.LineCode, ct)
                }); ;
            }

            return reportLines.DistinctBy(d => d.LineCode).ToList();
        }

        async Task<List<InventoryAdjustmentsViewModel.Item>> GetLineItems(int adjustmentId, int warehouseId, string lineCode, CancellationToken ct = default)
        {
            var reportItems = new List<InventoryAdjustmentsViewModel.Item>();

            foreach (var item in dataReport.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId && w.LineCode.Equals(lineCode)).Select(s => new { s.ItemName, s.InternalReference }).DistinctBy(d => new { d.ItemName, d.InternalReference }))
            {
                reportItems.Add(new InventoryAdjustmentsViewModel.Item
                {
                    ItemName = item.ItemName,
                    InternalReference = item.InternalReference,
                    References = await GetItemReferences(adjustmentId, warehouseId, item.ItemName, ct)
                }); ; ;
            }

            return reportItems.DistinctBy(d => d.ItemName).ToList();
        }

        async Task<List<InventoryAdjustmentsViewModel.Reference>> GetItemReferences(int adjustmentId, int warehouseId, string itemName, CancellationToken ct = default)
        {
            var reportReferences = new List<InventoryAdjustmentsViewModel.Reference>();

            foreach (var reference in dataReport.Where(w => w.AdjustmentId == adjustmentId && w.WarehouseId == warehouseId && w.ItemName.Equals(itemName)).Select(s => new { s.ReferenceName, s.ReferenceCode, s.AvailableAmount }).DistinctBy(d => new { d.ReferenceCode }))
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
