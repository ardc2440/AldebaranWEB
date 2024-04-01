using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class AddForwarder : ComponentBase
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        public IForwarderService ForwarderService { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.Forwarder Forwarder;
        protected bool IsSubmitInProgress;
        protected List<string> ValidationErrors;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Forwarder = new ServiceModel.Forwarder();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationErrors = new List<string>();
                var forwarderNameAlreadyExists = await ForwarderService.ExistsByForwarderName(Forwarder.ForwarderName);
                if (forwarderNameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe una transportadora con el mismo nombre.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }
                await ForwarderService.AddAsync(Forwarder);
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