using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages
{
    public partial class EditCustomerContact
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

        [Parameter]
        public int CUSTOMER_CONTACT_ID { get; set; }

        [Parameter]
        public int CUSTOMER_ID { get; set; }

        protected bool errorVisible;
        protected CustomerContact customerContact;
        protected City city;
        protected Customer customer;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            customerContact = await AldebaranDbService.GetCustomerContactByCustomerContactId(CUSTOMER_CONTACT_ID);
            customer = await AldebaranDbService.GetCustomerByCustomerId(customerContact.CUSTOMER_ID);
            var selectedCity = await AldebaranDbService.GetCities(new Query { Filter = "i=>i.CITY_ID == @0", FilterParameters = new object[] { customer.CITY_ID }, Expand = "Department.Country" });
            city = selectedCity.Single();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateCustomerContact(CUSTOMER_CONTACT_ID, customerContact);
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