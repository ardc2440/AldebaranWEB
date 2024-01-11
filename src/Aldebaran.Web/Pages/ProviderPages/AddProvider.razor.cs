using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class AddProvider
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        public IProviderService ProviderService { get; set; }
        [Inject]
        public IIdentityTypeService IdentityTypeService { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.Provider Provider;
        protected IEnumerable<ServiceModel.IdentityType> IdentityTypes;
        protected bool IsSubmitInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Provider = new ServiceModel.Provider();
            IdentityTypes = await IdentityTypeService.GetAsync();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await ProviderService.AddAsync(Provider);
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