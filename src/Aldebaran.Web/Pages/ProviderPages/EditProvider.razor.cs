using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class EditProvider
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

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public int PROVIDER_ID { get; set; }

        protected bool errorVisible;
        protected Models.AldebaranDb.Provider provider;
        protected IEnumerable<Models.AldebaranDb.IdentityType> identityTypesForIDENTITYTYPEID;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            provider = await AldebaranDbService.GetProviderByProviderId(PROVIDER_ID);
            identityTypesForIDENTITYTYPEID = await AldebaranDbService.GetIdentityTypes();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateProvider(PROVIDER_ID, provider);
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
        protected async Task LocalizationHandler(ServiceModel.City city)
        {
            provider.CITY_ID = city?.CityId ?? 0;
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}