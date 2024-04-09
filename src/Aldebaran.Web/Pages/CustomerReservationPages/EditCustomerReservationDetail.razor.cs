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

        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;                       
        protected string Error;
        protected CustomerReservationDetail CustomerReservationDetailData { get; set; }

        protected ItemReference ItemReference { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                CustomerReservationDetailData = new CustomerReservationDetail
                {
                    Brand = CustomerReservationDetail.Brand,
                    CustomerReservationDetailId = CustomerReservationDetail.CustomerReservationDetailId,
                    ReferenceId = CustomerReservationDetail.ReferenceId,
                    ItemReference = CustomerReservationDetail.ItemReference,
                    ReservedQuantity = CustomerReservationDetail.ReservedQuantity,
                    CustomerReservation = CustomerReservationDetail.CustomerReservation,
                    SendToCustomerOrder = CustomerReservationDetail.SendToCustomerOrder,
                    CustomerReservationId = CustomerReservationDetail.CustomerReservationId
                };

                ItemReference = await ItemReferenceService.FindAsync(CustomerReservationDetail.ReferenceId);
            }
            finally
            {
                isLoadingInProgress = false;
            }

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
                IsErrorVisible = false;
                IsSubmitInProgress = true;
                DialogService.Close(CustomerReservationDetailData);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}