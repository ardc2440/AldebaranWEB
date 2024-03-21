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

        #endregion

        #region Variables
        protected InventoryAdjustmentsFilter Filter;
        protected InventoryAdjustmentsViewModel ViewModel;
        protected IEnumerable<Warehouse> warehouses;
        protected IEnumerable<ItemReference> itemReferences;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            warehouses = await WarehouseService.GetAsync();

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

            foreach (var adjustment in await AdjustmentService.GetAsync(ct))
            {
                adjustments.Add(new InventoryAdjustmentsViewModel.Adjustment
                {
                    AdjustmentDate = adjustment.AdjustmentDate,
                    AdjustmentId = adjustment.AdjustmentId,
                    AdjustmentReason = adjustment.AdjustmentReason.AdjustmentReasonName,
                    AdjustmentType = adjustment.AdjustmentType.AdjustmentTypeName,
                    CreationDate = adjustment.CreationDate,
                    Employee = adjustment.Employee.FullName,
                    Notes = adjustment.Notes,
                    Warehouses = await GetAdjustmentsAsync(adjustment, ct)
                });
            }

            return adjustments;
        }

        async Task<List<InventoryAdjustmentsViewModel.Warehouse>> GetAdjustmentsAsync(Adjustment adjustment, CancellationToken ct = default)
        {
            var warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>();

            foreach (var warehouseId in adjustment.AdjustmentDetails.DistinctBy(d => d.WarehouseId).Select(s => s.WarehouseId))
            {
                var warehouse = await WarehouseService.FindAsync(warehouseId, ct);

                warehouses.Add(new InventoryAdjustmentsViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Lines = await GetWarehouseLines(adjustment, warehouse.WarehouseId, ct)
                });
            }

            return warehouses;
        }

        async Task<List<InventoryAdjustmentsViewModel.Line>> GetWarehouseLines(Adjustment adjustment, int warehouseId, CancellationToken ct = default)
        {
            var reportLines = new List<InventoryAdjustmentsViewModel.Line>();

            var adjustmentDetailbyWarehouse = adjustment.AdjustmentDetails.Where(w => w.WarehouseId == warehouseId);

            foreach (var referenceId in adjustmentDetailbyWarehouse.DistinctBy(d => d.ReferenceId).Select(s => s.ReferenceId))
            {
                var referece = await ItemReferenceService.FindAsync(referenceId, ct);

                reportLines.Add(new InventoryAdjustmentsViewModel.Line
                {
                    LineCode = referece.Item.Line.LineCode,
                    LineName = referece.Item.Line.LineName,
                    Items = await GetLineItems(adjustmentDetailbyWarehouse, ct)
                }); ;
            }

            return reportLines.DistinctBy(d => d.LineCode).ToList();
        }

        async Task<List<InventoryAdjustmentsViewModel.Item>> GetLineItems(IEnumerable<AdjustmentDetail> adjustmentDetails, CancellationToken ct = default)
        {
            var reportItems = new List<InventoryAdjustmentsViewModel.Item>();

            foreach (var referenceId in adjustmentDetails.DistinctBy(d => d.ReferenceId).Select(s => s.ReferenceId))
            {
                var referece = await ItemReferenceService.FindAsync(referenceId, ct);

                reportItems.Add(new InventoryAdjustmentsViewModel.Item
                {
                    ItemName = referece.Item.ItemName,
                    InternalReference = referece.Item.InternalReference,
                    References = await GetItemReferences(adjustmentDetails, referece.ItemId, ct)
                }); ; ;
            }

            return reportItems.DistinctBy(d => d.ItemName).ToList();
        }

        async Task<List<InventoryAdjustmentsViewModel.Reference>> GetItemReferences(IEnumerable<AdjustmentDetail> adjustmentDetails, int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<InventoryAdjustmentsViewModel.Reference>();

            foreach (var adjustmentDetail in adjustmentDetails)
            {
                var referece = await ItemReferenceService.FindAsync(adjustmentDetail.ReferenceId, ct);

                if (referece.ItemId == itemId)
                {
                    reportReferences.Add(new InventoryAdjustmentsViewModel.Reference
                    {
                        ReferenceName = referece.ReferenceName,
                        ReferenceCode = referece.ReferenceCode,
                        AvailableAmount = adjustmentDetail.Quantity
                    });
                }
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
