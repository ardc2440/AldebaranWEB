using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages
{
    public partial class AddCustomerContact
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
        protected CustomerContact customerContact;
        protected Customer customer;
        protected City city;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            customer = await AldebaranDbService.GetCustomerByCustomerId(CUSTOMER_ID);
            var selectedCity = await AldebaranDbService.GetCities(new Query { Filter = "i=>i.CITY_ID == @0", FilterParameters = new object[] { customer.CITY_ID }, Expand = "Department.Country" });
            city = selectedCity.Single();
            customerContact = new CustomerContact();
            customerContact.CUSTOMER_ID = CUSTOMER_ID;
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateCustomerContact(customerContact);
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
    }
}