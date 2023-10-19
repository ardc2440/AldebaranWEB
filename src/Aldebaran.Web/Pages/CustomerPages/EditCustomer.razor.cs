using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerPages
{
    public partial class EditCustomer
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public int CUSTOMER_ID { get; set; }
        protected bool errorVisible;
        protected Models.AldebaranDb.Customer customer;
        protected IEnumerable<Models.AldebaranDb.City> citiesForCITYID;
        protected IEnumerable<Models.AldebaranDb.IdentityType> identityTypesForIDENTITYTYPEID;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            customer = await AldebaranDbService.GetCustomerByCustomerId(CUSTOMER_ID);
            identityTypesForIDENTITYTYPEID = await AldebaranDbService.GetIdentityTypes();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateCustomer(CUSTOMER_ID, customer);
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

        protected async Task LocalizationHandler(Models.AldebaranDb.City city)
        {
            customer.CITY_ID = city?.CITY_ID ?? 0;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}