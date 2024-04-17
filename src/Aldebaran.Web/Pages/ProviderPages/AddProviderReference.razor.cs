using Aldebaran.Application.Services;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ProviderPages
{
    public partial class AddProviderReference
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IProviderReferenceService ProviderReferenceService { get; set; }
        [Inject]
        protected IProviderService ProviderService { get; set; }
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected IItemService ItemService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int PROVIDER_ID { get; set; }
        #endregion

        #region Variables
        public List<ServiceModel.ItemReference> AvailableItemReferencesForSelection { get; set; } = new List<ServiceModel.ItemReference>();
        protected ServiceModel.ProviderReference ProviderReference;
        protected ServiceModel.Provider Provider;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        protected List<ServiceModel.ItemReference> SelectedReferences = new();
        protected MultiReferencePicker referencePicker;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Provider = await ProviderService.FindAsync(PROVIDER_ID);
                var itemReferences = await ItemReferenceService.GetAsync();
                var currentReferencesInProvider = await ProviderReferenceService.GetByProviderIdAsync(PROVIDER_ID);
                // Referencias disponibles para seleccion, Referencias excepto los ya seleccionados
                AvailableItemReferencesForSelection = itemReferences.Where(w => !currentReferencesInProvider.Any(x => x.ReferenceId == w.ReferenceId)).ToList();
                ProviderReference = new ServiceModel.ProviderReference
                {
                    ProviderId = PROVIDER_ID
                };
            }
            finally
            {
                isLoadingInProgress = false;
                StateHasChanged();
                referencePicker?.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                var pr = SelectedReferences.Select(s => new ServiceModel.ProviderReference
                {
                    ProviderId = ProviderReference.ProviderId,
                    ReferenceId = s.ReferenceId
                }).ToList();
                await ProviderReferenceService.AddRangeAsync(pr);
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
        protected async Task ItemReferenceHandler(List<ServiceModel.ItemReference> references)
        {
            SelectedReferences = references ?? new List<ServiceModel.ItemReference>();
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}