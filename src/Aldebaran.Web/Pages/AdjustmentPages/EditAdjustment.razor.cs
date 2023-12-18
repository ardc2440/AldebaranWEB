using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class EditAdjustment
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IAdjustmentReasonService AdjustmentReasonService { get; set; }

        [Inject]
        protected IAdjustmentTypeService AdjustmentTypeService { get; set; }

        [Inject]
        protected IAdjustmentDetailService AdjustmentDetailService { get; set; }

        [Inject]
        protected IAdjustmentService AdjustmentService { get; set; }

        #endregion

        #region Global Variables

        [Parameter]
        public string AdjustmentId { get; set; } = "NoParamInput";
        protected DateTime Now { get; set; }

        protected bool errorVisible;

        protected string errorMessage;

        protected Adjustment adjustment;

        protected IEnumerable<AdjustmentReason> adjustmentReasonsForADJUSTMENTREASONID;

        protected IEnumerable<AdjustmentType> adjustmentTypesForADJUSTMENTTYPEID;

        protected IEnumerable<Employee> employeesForEMPLOYEEID;

        protected ICollection<AdjustmentDetail> adjustmentDetails;

        protected RadzenDataGrid<AdjustmentDetail> adjustmentDetailGrid;

        protected bool isSubmitInProgress;

        protected RadzenPanelMenu panelMenu;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            adjustmentReasonsForADJUSTMENTREASONID = await AdjustmentReasonService.GetAsync();

            adjustmentTypesForADJUSTMENTTYPEID = await AdjustmentTypeService.GetAsync();

            Now = DateTime.UtcNow.AddDays(-1);

            adjustmentDetails = new List<AdjustmentDetail>();

            var adjustmentId = 0;

            int.TryParse(AdjustmentId, out adjustmentId);

            adjustment = await AdjustmentService.FindAsync(adjustmentId);

            await GetChildData(adjustment);
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!adjustmentDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                adjustment.AdjustmentDetails = adjustmentDetails;
                await AdjustmentService.UpdateAsync(adjustment.AdjustmentId, adjustment);
                await DialogService.Alert("Ajuste Modificado Satisfactoriamente", "Información");
                NavigationManager.NavigateTo("adjustments");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar la modificación del Ajuste??", "Confirmar") == true)
                NavigationManager.NavigateTo("adjustments");
        }

        protected async Task AddAdjustmentDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAdjustmentDetail>("Nueva referencia", new Dictionary<string, object> { { "adjustmentDetails", adjustmentDetails } });

            if (result == null)
                return;

            var detail = (AdjustmentDetail)result;

            adjustmentDetails.Add(detail);

            await adjustmentDetailGrid.Reload();
        }

        protected async Task DeleteAdjustmentDetailButtonClick(MouseEventArgs args, AdjustmentDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                adjustmentDetails.Remove(item);

                await adjustmentDetailGrid.Reload();
            }
        }

        protected async Task EditRow(AdjustmentDetail args)
        {
            var result = await DialogService.OpenAsync<EditAdjustmentDetail>("Actualizar referencia", new Dictionary<string, object> { { "AdjustmentDetail", args } });
            if (result == null)
                return;
            var detail = (AdjustmentDetail)result;

            args.Quantity = detail.Quantity;
            await adjustmentDetailGrid.Reload();
        }

        protected async Task GetChildData(Adjustment args)
        {
            var AdjustmentDetailsResult = await AdjustmentDetailService.GetByAdjustmentIdAsync(args.AdjustmentId);
            if (AdjustmentDetailsResult != null)
            {
                adjustmentDetails = AdjustmentDetailsResult.ToList();
            }
        }

        #endregion
    }
}