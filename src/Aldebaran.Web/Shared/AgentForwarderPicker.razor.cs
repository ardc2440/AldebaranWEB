using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class AgentForwarderPicker
    {
        #region Injections
        [Inject]
        protected IForwarderService ForwarderService { get; set; }
        [Inject]
        protected IForwarderAgentService ForwarderAgentService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public EventCallback<ServiceModel.ForwarderAgent> OnChange { get; set; }
        #endregion

        #region Variables
        protected bool CollapsedPanel { get; set; } = true;
        protected ServiceModel.Forwarder Forwarder;
        protected ServiceModel.ForwarderAgent ForwarderAgent;
        protected IEnumerable<ServiceModel.Forwarder> Forwarders;
        protected IEnumerable<ServiceModel.ForwarderAgent> ForwarderAgents;
        public int? FORWARDER_ID { get; set; }
        public int? FORWARDER_AGENT_ID { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            Forwarders = await ForwarderService.GetAsync();
        }
        #endregion

        #region Events
        protected async Task OnForwarderChange(object forwarderId)
        {
            if (forwarderId == null)
            {
                Forwarder = null;
                ForwarderAgent = null;
                ForwarderAgents = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            Forwarder = Forwarders.Single(s => s.ForwarderId == (int)forwarderId);
            ForwarderAgents = await ForwarderAgentService.GetByForwarderIdAsync(Forwarder.ForwarderId);
        }
        protected async Task OnForwarderAgentChange(object forwarderAgentId)
        {
            if (forwarderAgentId == null)
            {
                ForwarderAgent = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            ForwarderAgent = ForwarderAgents.Single(s => s.ForwarderAgentId == (int)forwarderAgentId);
            CollapsedPanel = true;
            await OnChange.InvokeAsync(ForwarderAgent);
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
        #endregion
    }
}