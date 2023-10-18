using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class EditForwarderAgent
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
        public int FORWARDER_AGENT_ID { get; set; }

        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.ForwarderAgent forwarderAgent;
        protected Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder;
        protected Aldebaran.Web.Models.AldebaranDb.City city;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            forwarderAgent = await AldebaranDbService.GetForwarderAgentByForwarderAgentId(FORWARDER_AGENT_ID);
            forwarder = await AldebaranDbService.GetForwarderByForwarderId(forwarderAgent.FORWARDER_ID);
            var selectedCity = await AldebaranDbService.GetCities(new Query { Filter = "i=>i.CITY_ID == @0", FilterParameters = new object[] { forwarder.CITY_ID }, Expand = "Department.Country" });
            city = selectedCity.Single();
        }
        
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateForwarderAgent(FORWARDER_AGENT_ID, forwarderAgent);
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

        protected async Task LocalizationHandler(Aldebaran.Web.Models.AldebaranDb.City city)
        {
            forwarderAgent.CITY_ID = city?.CITY_ID ?? 0;
        }
    }
}