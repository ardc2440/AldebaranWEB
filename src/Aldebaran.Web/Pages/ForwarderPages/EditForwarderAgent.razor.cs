using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

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

        [Parameter]
        public int FORWARDER_AGENT_ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            forwarderAgent = await AldebaranDbService.GetForwarderAgentByForwarderAgentId(FORWARDER_AGENT_ID);

            citiesForCITYID = await AldebaranDbService.GetCities();

            forwardersForFORWARDERID = await AldebaranDbService.GetForwarders();
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.ForwarderAgent forwarderAgent;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.City> citiesForCITYID;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Forwarder> forwardersForFORWARDERID;
        protected bool isSubmitInProgress;
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


        bool hasCITY_IDValue;

        [Parameter]
        public int CITY_ID { get; set; }

        bool hasFORWARDER_IDValue;

        [Parameter]
        public int FORWARDER_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            forwarderAgent = new Aldebaran.Web.Models.AldebaranDb.ForwarderAgent();

            hasCITY_IDValue = parameters.TryGetValue<int>("CITY_ID", out var hasCITY_IDResult);

            if (hasCITY_IDValue)
            {
                forwarderAgent.CITY_ID = hasCITY_IDResult;
            }

            hasFORWARDER_IDValue = parameters.TryGetValue<int>("FORWARDER_ID", out var hasFORWARDER_IDResult);

            if (hasFORWARDER_IDValue)
            {
                forwarderAgent.FORWARDER_ID = hasFORWARDER_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}