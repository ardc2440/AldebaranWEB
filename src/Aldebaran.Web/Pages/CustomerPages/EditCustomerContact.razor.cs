using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class EditCustomerContact
    {
        #region Injections
        [Inject]
        protected ILogger<EditCustomerContact> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerContactService CustomerContactService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected ICityService CityService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public int CUSTOMER_CONTACT_ID { get; set; }

        [Parameter]
        public int CUSTOMER_ID { get; set; }

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        protected CustomerContact CustomerContact;
        protected City CustomerCity;
        protected Customer Customer;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                CustomerContact = await CustomerContactService.FindAsync(CUSTOMER_CONTACT_ID);
                Customer = await CustomerService.FindAsync(CustomerContact.CustomerId);
                CustomerCity = await CityService.FindAsync(Customer.CityId);
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await CustomerContactService.UpdateAsync(CUSTOMER_CONTACT_ID, CustomerContact);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
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