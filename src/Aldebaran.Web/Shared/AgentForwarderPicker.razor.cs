using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Shared
{
    public partial class AgentForwarderPicker
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
        protected SecurityService Security { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Parameter]
        public EventCallback<Models.AldebaranDb.ForwarderAgent> OnChange { get; set; }

        protected bool CollapsedPanel { get; set; } = true;
        protected Models.AldebaranDb.Forwarder forwarder;
        protected Models.AldebaranDb.ForwarderAgent agent;
        protected IEnumerable<Models.AldebaranDb.Forwarder> forwarders;
        protected IEnumerable<Models.AldebaranDb.ForwarderAgent> agents;
        public int? FORWARDER_ID { get; set; }
        public int? FORWARDER_AGENT_ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            forwarders = await AldebaranDbService.GetForwarders(new Query { Expand = "City.Department.Country" });
        }
        protected async Task OnForwarderChange(object forwarderId)
        {
            if (forwarderId == null)
            {
                forwarder = null;
                await CleanAgents();
                return;
            }
            forwarder = forwarders.Single(s => s.FORWARDER_ID == (int)forwarderId);
            agents = await AldebaranDbService.GetForwarderAgents(new Query { Filter = $"i=>i.FORWARDER_ID==@0", FilterParameters = new object[] { forwarderId }, Expand = "City.Department.Country" });
        }
        protected async Task OnForwarderAgentChange(object forwarderAgentId)
        {
            if (forwarderAgentId == null)
            {
                agent = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            agent = agents.Single(s => s.FORWARDER_AGENT_ID == (int)forwarderAgentId);
            CollapsedPanel = true;
            await OnChange.InvokeAsync(agent);
        }
        async Task CleanAgents()
        {
            agent = null;
            agents = null;
            await OnChange.InvokeAsync(null);
        }
        protected async Task PanelCollapseToggle(MouseEventArgs args)
        {
            CollapsedPanel = !CollapsedPanel;
        }
        void PanelCollapseChange(string Command)
        {
            if (Command == "Expand")
                CollapsedPanel = false;
            if (Command == "Collapse")
                CollapsedPanel = true;
        }
    }
}