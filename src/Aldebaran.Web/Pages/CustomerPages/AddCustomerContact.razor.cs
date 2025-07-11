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
        protected ILogger<AddCustomerContact> Logger { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected ICustomerService CustomerService { get; set; }
        [Inject]
        protected ICityService CityService { get; set; }
        [Inject]
        protected ICustomerContactService CustomerContactService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public int CUSTOMER_ID { get; set; }

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        protected CustomerContact CustomerContact;
        protected Customer Customer;
        protected City CustomerCity;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected List<string> ValidationErrors;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Customer = await CustomerService.FindAsync(CUSTOMER_ID);
                CustomerCity = await CityService.FindAsync(Customer.CityId);
                CustomerContact = new CustomerContact
                {
                    CustomerId = CUSTOMER_ID
                };
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
                ValidationErrors = new List<string>();
                var contactNameAlreadyExists = await CustomerContactService.ExistsByContactName(CustomerContact.CustomerId, CustomerContact.CustomerContactName);
                if (contactNameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un contacto con el mismo nombre.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }
                await CustomerContactService.AddAsync(CustomerContact);
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