using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class EditProvider
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        public IProviderService ProviderService { get; set; }
        [Inject]
        public IIdentityTypeService IdentityTypeService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int PROVIDER_ID { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.Provider Provider;
        protected IEnumerable<ServiceModel.IdentityType> IdentityTypes;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Provider = await ProviderService.FindAsync(PROVIDER_ID);
                IdentityTypes = await IdentityTypeService.GetAsync();
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
                await ProviderService.UpdateAsync(PROVIDER_ID, Provider);
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
            Provider.CityId = city?.CityId ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}