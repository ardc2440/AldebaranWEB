using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class EditAdjustment
    {
        #region Injections

        [Inject]
        protected ILogger<EditAdjustment> Logger { get; set; }

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

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string AdjustmentId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected DateTime Now { get; set; }

        protected Adjustment adjustment;
        protected IEnumerable<AdjustmentReason> adjustmentReasonsForADJUSTMENTREASONID;
        protected IEnumerable<AdjustmentType> adjustmentTypesForADJUSTMENTTYPEID;
        protected IEnumerable<Employee> employeesForEMPLOYEEID;
        protected ICollection<AdjustmentDetail> adjustmentDetails;
        protected LocalizedDataGrid<AdjustmentDetail> adjustmentDetailGrid;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected string Error;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            if (AdjustmentId == null)
                NavigationManager.NavigateTo("adjustments");
            var valid = int.TryParse(AdjustmentId, out var adjustmentId);
            if (!valid)
                NavigationManager.NavigateTo("purchase-orders");

            adjustmentReasonsForADJUSTMENTREASONID = await AdjustmentReasonService.GetAsync();
            adjustmentTypesForADJUSTMENTTYPEID = await AdjustmentTypeService.GetAsync();
            adjustmentDetails = new List<AdjustmentDetail>();

            adjustment = await AdjustmentService.FindAsync(adjustmentId);

            await GetChildData(adjustment);
        }

        #endregion

        #region Events

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task FormSubmit()
        {
            try
            {
                Submitted = true;
                IsSubmitInProgress = true;
                if (!adjustmentDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                adjustment.AdjustmentDetails = adjustmentDetails;
                await AdjustmentService.UpdateAsync(adjustment.AdjustmentId, adjustment);
                NavigationManager.NavigateTo($"adjustments/edit/{adjustment.AdjustmentId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Est� seguro que desea cancelar la modificaci�n del ajuste?", "Confirmar") == true)
                NavigationManager.NavigateTo("adjustments");
        }

        protected async Task AddAdjustmentDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAdjustmentDetail>("Agregar referencia", new Dictionary<string, object> { { "adjustmentDetails", adjustmentDetails } });

            if (result == null)
                return;

            var detail = (AdjustmentDetail)result;

            adjustmentDetails.Add(detail);

            await adjustmentDetailGrid.Reload();
        }

        protected async Task DeleteAdjustmentDetailButtonClick(MouseEventArgs args, AdjustmentDetail item)
        {
            if (await DialogService.Confirm("Est� seguro que desea eliminar esta referencia?", "Confirmar") == true)
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