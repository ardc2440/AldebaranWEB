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
        protected IItemReferenceService ItemReferenceService { get; set; }

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
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected IEnumerable<Provider> Providers;
        protected MultiReferencePicker referencePicker;
        protected RadzenDropDownDataGrid<int?> providersDropdown;
        protected bool ValidationError = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new ProviderReferencesFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            Providers = (await ProviderService.GetAsync()).ToList();
        }
        protected bool FirstRender = true;
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (FirstRender == false) return;
            if (Filter?.ItemReferences?.Any() == true)
            {
                referencePicker.SetSelectedItemReferences(Filter.ItemReferences.Select(s => s.ReferenceId).ToList());
            }
            FirstRender = false;
            StateHasChanged();
        }
        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                // Si no se han incluido filtros, mostrar mensaje de error
                if (Filter.ProviderId == null && SelectedReferences.Any() == false)
                {
                    ValidationError = true;
                    return;
                }
                Filter.Provider = Filter.ProviderId != null ? Providers.Single(s => s.ProviderId == Filter.ProviderId.Value) : null;
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
        #endregion
    }
}
