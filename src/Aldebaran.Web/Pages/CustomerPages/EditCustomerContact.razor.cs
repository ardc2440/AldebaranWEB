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

        protected bool errorVisible;
        protected CustomerContact customerContact;
        protected City city;
        protected Customer customer;
        protected bool isSubmitInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            customerContact = await CustomerContactService.FindAsync(CUSTOMER_CONTACT_ID);
            customer = await CustomerService.FindAsync(customerContact.CustomerId);
            city = await CityService.FindAsync(customer.CityId);
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await CustomerContactService.UpdateAsync(CUSTOMER_CONTACT_ID, customerContact);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
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