using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Provider_References.Components
{
    public partial class ProviderReferencesReportFilter
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected IProviderReferenceService ProviderReferenceService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public ProviderReferencesFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected List<ItemReference> SelectedReferences = new();
        protected IEnumerable<Provider> Providers;
        protected MultiReferencePicker referencePicker;
        public bool AllowItemReferenceSelection { get; set; } = false;
        protected int? ProviderId;
        protected RadzenDropDownDataGrid<int?> providersDropdown;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new ProviderReferencesFilter();
            Providers = await ProviderService.GetAsync();
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (Filter?.Provider != null)
            {
                ProviderId = Filter.Provider.ProviderId;
                var provider = Providers.Single(s => s.ProviderId == ProviderId);
                await providersDropdown.DataGrid.SelectRow(provider, false);
                var providerReferences = await ProviderReferenceService.GetByProviderIdAsync(Filter.Provider.ProviderId);
                referencePicker.SetAvailableItemReferencesForSelection(providerReferences.Select(s => s.ItemReference).ToList());
                referencePicker.SetSelectedItemReferences(Filter.ItemReferences.Select(s => s.ReferenceId).ToList());
                AllowItemReferenceSelection = true;
            }
            else
            {
                AllowItemReferenceSelection = false;
                referencePicker.SetAvailableItemReferencesForSelection(null);
                referencePicker.SetSelectedItemReferences(null);
            }
            StateHasChanged();
        }
        #region Events
        protected async Task FormSubmit()
        {
            try
            {

                IsSubmitInProgress = true;
                Filter.Provider = Providers.Single(s => s.ProviderId == ProviderId.Value);
                Filter.ItemReferences = SelectedReferences;
                DialogService.Close(Filter);
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
        protected async Task ItemReferenceHandler(List<ItemReference> references)
        {
            SelectedReferences = references ?? new List<ItemReference>();
        }
        protected async Task ProviderSelectionChange(object providerId)
        {
            var id = (int?)providerId;
            if (id == null)
            {
                AllowItemReferenceSelection = false;
                referencePicker.SetAvailableItemReferencesForSelection(null);
                referencePicker.SetSelectedItemReferences(null);
                StateHasChanged();
                return;
            }
            var providerReferences = await ProviderReferenceService.GetByProviderIdAsync(id.Value);
            referencePicker.SetAvailableItemReferencesForSelection(providerReferences.Select(s => s.ItemReference).ToList());
            referencePicker.SetSelectedItemReferences(null);
            AllowItemReferenceSelection = true;
            StateHasChanged();
        }
        #endregion
    }
}
