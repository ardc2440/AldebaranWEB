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