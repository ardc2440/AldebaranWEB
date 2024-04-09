using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class AddForwarderAgent : ComponentBase
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IForwarderService ForwarderService { get; set; }
        [Inject]
        protected IForwarderAgentService ForwarderAgentService { get; set; }
        [Inject]
        protected ICityService CityService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int FORWARDER_ID { get; set; }
        #endregion

        #region Variables
        protected ServiceModel.ForwarderAgent ForwarderAgent;
        protected ServiceModel.Forwarder Forwarder;
        protected ServiceModel.City SelectedCity;
        protected bool IsSubmitInProgress;
        protected bool IsErrorVisible;
        protected bool isLoadingInProgress;
        protected List<string> ValidationErrors;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Forwarder = await ForwarderService.FindAsync(FORWARDER_ID);
                SelectedCity = await CityService.FindAsync(Forwarder.CityId);
                ForwarderAgent = new ServiceModel.ForwarderAgent
                {
                    ForwarderId = FORWARDER_ID
                };
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationErrors = new List<string>();
                var agentNameAlreadyExists = await ForwarderAgentService.ExistsByAgentName(ForwarderAgent.ForwarderId, ForwarderAgent.ForwarderAgentName);
                if (agentNameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un agente con el mismo nombre.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }
                await ForwarderAgentService.AddAsync(ForwarderAgent);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task LocalizationHandler(ServiceModel.City city)
        {
            ForwarderAgent.CityId = city?.CityId ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}