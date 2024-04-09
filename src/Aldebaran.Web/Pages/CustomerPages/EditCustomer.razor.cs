using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class EditCustomer
    {
        #region Injections
        [Inject]
        protected ILogger<EditCustomer> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IIdentityTypeService IdentityTypeService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public int CUSTOMER_ID { get; set; }

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        protected Customer Customer;
        protected IEnumerable<IdentityType> IdentityTypesForSelection = new List<IdentityType>();
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Customer = await CustomerService.FindAsync(CUSTOMER_ID);
                IdentityTypesForSelection = await IdentityTypeService.GetAsync();
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
                await CustomerService.UpdateAsync(CUSTOMER_ID, Customer);
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