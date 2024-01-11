using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class EditForwarderAgent : ComponentBase
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
        public int FORWARDER_AGENT_ID { get; set; }
        #endregion

        #region Variables
        protected ServiceModel.ForwarderAgent ForwarderAgent;
        protected ServiceModel.Forwarder Forwarder;
        protected ServiceModel.City SelectedCity;
        protected bool IsSubmitInProgress;
        protected bool IsErrorVisible;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ForwarderAgent = await ForwarderAgentService.FindAsync(FORWARDER_AGENT_ID);
            Forwarder = await ForwarderService.FindAsync(ForwarderAgent.ForwarderId);
            SelectedCity = await CityService.FindAsync(Forwarder.CityId);
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await ForwarderAgentService.UpdateAsync(FORWARDER_AGENT_ID, ForwarderAgent);
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
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        protected async Task LocalizationHandler(ServiceModel.City city)
        {
            ForwarderAgent.CityId = city?.CityId ?? 0;
        }
        #endregion
    }
}