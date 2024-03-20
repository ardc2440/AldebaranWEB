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
        protected IAdjustmentService AdjustmentService { get; set; }
        #endregion

        #region Variables
        protected InventoryAdjustmentsFilter Filter;
        protected InventoryAdjustmentsViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new InventoryAdjustmentsViewModel()
            {
                Adjustments = (await GetAdjustmentsAsync()).ToList()
            };
        }
        #endregion

        #region Fill Data Report

        async Task<IEnumerable<InventoryAdjustmentsViewModel.Adjustment>> GetAdjustmentsAsync(CancellationToken ct = default)
        {
            return from adjustment in await AdjustmentService.GetAsync(ct)
                   select new InventoryAdjustmentsViewModel.Adjustment
                   {
                       AdjustmentDate = adjustment.AdjustmentDate,
                       AdjustmentId = adjustment.AdjustmentId,
                       AdjustmentReason = adjustment.AdjustmentReason.AdjustmentReasonName,
                       AdjustmentType = adjustment.AdjustmentType.AdjustmentTypeName,
                       CreationDate = adjustment.CreationDate,
                       Employee = adjustment.Employee.FullName,
                       Notes = adjustment.Notes,
                       Warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>()
                   };
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
