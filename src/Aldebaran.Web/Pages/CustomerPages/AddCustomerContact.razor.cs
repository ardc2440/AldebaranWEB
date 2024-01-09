using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class AddCustomerContact
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected ICustomerService CustomerService { get; set; }
        [Inject]
        protected ICityService CityService { get; set; }
        [Inject]
        protected ICustomerContactService CustomerContactService { get; }

        #endregion

        #region Parameters

        [Parameter]
        public int CUSTOMER_ID { get; set; }

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected CustomerContact customerContact;
        protected Customer customer;
        protected City city;
        protected bool isSubmitInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            customer = await CustomerService.FindAsync(CUSTOMER_ID);
            city = await CityService.FindAsync(customer.CityId);
            customerContact = new CustomerContact
            {
                CustomerId = CUSTOMER_ID
            };
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await CustomerContactService.AddAsync(customerContact);
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