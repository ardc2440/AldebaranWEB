using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class AddShipmentForwarderAgentMethod
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
        protected IEnumerable<ShipmentMethod> shipmentMethods;
        protected bool errorVisible;
        protected ForwarderAgent forwarderAgent;
        protected ShipmentForwarderAgentMethod shipmentForwarderAgentMethod;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            forwarderAgent = await AldebaranDbService.GetForwarderAgentByForwarderAgentId(FORWARDER_AGENT_ID);
            var currentShipmentMethodsForAgent = await AldebaranDbService.GetShipmentForwarderAgentMethods(new Query { Filter = $"@i => i.FORWARDER_AGENT_ID == @0", FilterParameters = new object[] { FORWARDER_AGENT_ID } });
            shipmentMethods = await AldebaranDbService.GetShipmentMethods(new Query { Filter = $"@i => !@0.Contains(i.SHIPMENT_METHOD_ID)", FilterParameters = new object[] { currentShipmentMethodsForAgent.Select(s => s.SHIPMENT_METHOD_ID) } });
            shipmentForwarderAgentMethod = new ShipmentForwarderAgentMethod();
            shipmentForwarderAgentMethod.FORWARDER_AGENT_ID = FORWARDER_AGENT_ID;
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateShipmentForwarderAgentMethod(shipmentForwarderAgentMethod);
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