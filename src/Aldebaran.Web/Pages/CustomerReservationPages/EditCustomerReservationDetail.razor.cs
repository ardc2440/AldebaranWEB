using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class EditCustomerReservationDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public CustomerReservationDetail CustomerReservationDetail { get; set; }

        #endregion

        #region Global Variables
        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected ItemReference ItemReference { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ItemReference = await ItemReferenceService.FindAsync(CustomerReservationDetail.ReferenceId);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            CustomerReservationDetail = new CustomerReservationDetail();

            await base.SetParametersAsync(parameters);
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;
                DialogService.Close(CustomerReservationDetail);
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}