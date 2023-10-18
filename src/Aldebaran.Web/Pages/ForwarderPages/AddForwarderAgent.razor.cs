using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class AddForwarderAgent
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
        public int FORWARDER_ID { get; set; }
        protected bool errorVisible;
        protected Models.AldebaranDb.ForwarderAgent forwarderAgent;
        protected Models.AldebaranDb.Forwarder forwarder;
        protected Models.AldebaranDb.City city;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            forwarder = await AldebaranDbService.GetForwarderByForwarderId(FORWARDER_ID);
            var selectedCity = await AldebaranDbService.GetCities(new Query { Filter = "i=>i.CITY_ID == @0", FilterParameters = new object[] { forwarder.CITY_ID }, Expand = "Department.Country" });
            city = selectedCity.Single();
            forwarderAgent = new Models.AldebaranDb.ForwarderAgent();
            forwarderAgent.FORWARDER_ID = FORWARDER_ID;
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateForwarderAgent(forwarderAgent);
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
            forwarderAgent.CITY_ID = city?.CITY_ID ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}