using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class EditForwarder : ComponentBase
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        public IForwarderService ForwarderService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int FORWARDER_ID { get; set; }
        #endregion

        #region Variables
        protected bool IsSubmitInProgress;
        protected bool IsErrorVisible;
        protected ServiceModel.Forwarder Forwarder;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Forwarder = await ForwarderService.FindAsync(FORWARDER_ID);
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await ForwarderService.UpdateAsync(FORWARDER_ID, Forwarder);
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
            Forwarder.CityId = city?.CityId ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}