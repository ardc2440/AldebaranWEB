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

        [Parameter]
        public int? FORWARDER_AGENT_ID { get; set; }
        #endregion

        #region Variables
        protected bool CollapsedPanel { get; set; } = true;
        protected ServiceModel.Forwarder Forwarder;
        protected ServiceModel.ForwarderAgent ForwarderAgent;
        protected IEnumerable<ServiceModel.Forwarder> Forwarders = new List<ServiceModel.Forwarder>();
        protected IEnumerable<ServiceModel.ForwarderAgent> ForwarderAgents = new List<ServiceModel.ForwarderAgent>();
        public int? FORWARDER_ID { get; set; }
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            Forwarders = await ForwarderService.GetAsync();
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            if (FORWARDER_AGENT_ID == null)
                return;
            var selectedForwarderAgent = await ForwarderAgentService.FindAsync(FORWARDER_AGENT_ID.Value);
            if (selectedForwarderAgent == null)
                return;
            FORWARDER_ID = selectedForwarderAgent.ForwarderId;
            await OnForwarderChange(selectedForwarderAgent.ForwarderId);
            await OnForwarderAgentChange(selectedForwarderAgent.ForwarderAgentId);
        }
        #endregion

        #region Events
        protected async Task OnForwarderChange(object forwarderId)
        {
            if (forwarderId == null)
            {
                Forwarder = null;
                ForwarderAgent = null;
                ForwarderAgents = new List<ServiceModel.ForwarderAgent>();
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
            IsSetParametersEnabled = false;
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