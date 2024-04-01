using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class AddCustomer
    {
        #region Injections
        [Inject]
        protected ILogger<AddCustomer> Logger { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IIdentityTypeService IdentityTypeService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        protected Customer Customer;
        protected IEnumerable<IdentityType> IdentityTypesForSelection = new List<IdentityType>();
        protected bool IsSubmitInProgress;
        protected List<string> ValidationErrors;
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            Customer = new Customer();
            IdentityTypesForSelection = await IdentityTypeService.GetAsync();
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationErrors = new List<string>();
                var customerNamelreadyExists = await CustomerService.ExistsByName(Customer.CustomerName);
                if (customerNamelreadyExists)
                {
                    ValidationErrors.Add("Ya existe un cliente con el mismo nombre.");
                }
                var identityNumberAlreadyExists = await CustomerService.ExistsByIdentificationNumber(Customer.IdentityNumber);
                if (identityNumberAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un cliente con el mismo número de identificación.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }
                await CustomerService.AddAsync(Customer);
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

        protected async Task LocalizationHandler(City city)
        {
            Customer.CityId = city?.CityId ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}