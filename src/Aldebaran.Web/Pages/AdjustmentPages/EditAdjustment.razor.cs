using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class EditAdjustment
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Parameter]
        public int ADJUSTMENT_ID { get; set; }

        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            adjustment = await AldebaranDbService.GetAdjustmentByAdjustmentId(ADJUSTMENT_ID);

            adjustmentReasonsForADJUSTMENTREASONID = await AldebaranDbService.GetAdjustmentReasons();

            adjustmentTypesForADJUSTMENTTYPEID = await AldebaranDbService.GetAdjustmentTypes();

            //aspnetusersForASPNETUSERID = await AldebaranDbService.GetAspnetusers();
        }
        protected bool errorVisible;
        protected Models.AldebaranDb.Adjustment adjustment;

        protected IEnumerable<Models.AldebaranDb.AdjustmentReason> adjustmentReasonsForADJUSTMENTREASONID;

        protected IEnumerable<Models.AldebaranDb.AdjustmentType> adjustmentTypesForADJUSTMENTTYPEID;

        protected IEnumerable<Models.AldebaranDb.Employee> employeesForEMPLOYEEID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;

                await AldebaranDbService.UpdateAdjustment(ADJUSTMENT_ID, adjustment);
                DialogService.Close(adjustment);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}