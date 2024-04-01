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
        protected List<string> ValidationErrors;
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
                ValidationErrors = new List<string>();
                var identityNumberAlreadyExists = await ProviderService.ExistsByIdentificationNumber(Provider.IdentityNumber);
                if (identityNumberAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un proveedor con el mismo número de identificación.");
                }
                var providerCodeAlreadyExists = await ProviderService.ExistsByCode(Provider.ProviderCode);
                if (providerCodeAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un proveedor con el mismo código.");
                }
                var providerNameAlreadyExists = await ProviderService.ExistsByName(Provider.ProviderName);
                if (providerNameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un proveedor con el mismo nombre.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }

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