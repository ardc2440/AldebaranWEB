using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class AddProviderReference
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
        protected Models.AldebaranDb.ProviderReference providerReference;
        protected Models.AldebaranDb.Provider provider;
        //protected IEnumerable<Models.AldebaranDb.ItemReference> itemReferencesForREFERENCEID;
        protected bool isSubmitInProgress;
        public IEnumerable<int> CurrentProviderReferencesIds { get; set; }

        protected override async Task OnInitializedAsync()
        {
            provider = await AldebaranDbService.GetProviderByProviderId(PROVIDER_ID);
            var currentReferencesInProvider = await AldebaranDbService.GetProviderReferences(new Query { Filter = $"@i => i.PROVIDER_ID == @0", FilterParameters = new object[] { PROVIDER_ID } });
            CurrentProviderReferencesIds = currentReferencesInProvider.Select(s => s.REFERENCE_ID);
            providerReference = new Models.AldebaranDb.ProviderReference();
            providerReference.PROVIDER_ID = PROVIDER_ID;
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateProviderReference(providerReference);
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
        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            providerReference.REFERENCE_ID = reference?.REFERENCE_ID ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}