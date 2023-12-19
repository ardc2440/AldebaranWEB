using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared;
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
        #endregion

        #region Parameters
        [Parameter]
        public CustomerReservationDetail CustomerReservationDetail { get; set; }

        #endregion

        #region Global Variables
        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected InventoryQuantities QuantitiesPanel;

        #endregion

        #region Overrides
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

        protected async Task ItemReferenceHandler()
        {
            await QuantitiesPanel.Refresh(CustomerReservationDetail.ItemReference);
        }
        #endregion
    }
}