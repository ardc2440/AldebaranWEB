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

        protected bool errorVisible;
        protected Customer customer;
        protected IEnumerable<IdentityType> identityTypesForIDENTITYTYPEID;
        protected bool isSubmitInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            customer = await CustomerService.FindAsync(CUSTOMER_ID);
            identityTypesForIDENTITYTYPEID = await IdentityTypeService.GetAsync();
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await CustomerService.UpdateAsync(CUSTOMER_ID, customer);
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

        protected async Task LocalizationHandler(City city)
        {
            customer.CityId = city?.CityId ?? 0;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}