using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement.Components
{
    public partial class ReferenceMovementReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public ReferenceMovementFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected MultiReferencePicker referencePicker;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await AvailableReferenceFilter();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (Filter?.ItemReferences?.Any() == true)
            {
                referencePicker.SetSelectedItemReferences(Filter.ItemReferences.Select(s => s.ReferenceId).ToList());
            }
            StateHasChanged();
        }
        #endregion

        #region Events
        protected async Task OnItemCheckboxClick()
        {
            Filter.ShowInactiveReferences = false;
            
            Filter?.ItemReferences.Clear();
            await AvailableReferenceFilter();
        }

        protected async Task OnReferenceCheckboxClick()
        {
            if (Filter.ShowInactiveItems)
            {
                Filter.ShowInactiveReferences = false;
                return;
            }

            Filter?.ItemReferences.Clear();
            await AvailableReferenceFilter();
        }

        protected async Task AvailableReferenceFilter()
        {
            Filter ??= new ReferenceMovementFilter();

            var references = new List<ItemReference>();

            if (Filter?.ItemReferences?.Any() == true && Filter.AllMovementCheckVisible) /* Muestra solo la referencia seleccionada en las alarmas*/
                references.Add(await ItemReferenceService.FindAsync(Filter.ItemReferences.FirstOrDefault().ReferenceId));
            else if ((bool)Filter?.ShowInactiveItems) /* Lista solo items inactivos */
                references = (await ItemReferenceService.GetReportsReferencesAsync(isItemActive: false)).ToList();
            else if ((bool)Filter?.ShowInactiveReferences) /* lista solo referencias inactivas y los items asociados a esas refrencias*/
                references = (await ItemReferenceService.GetReportsReferencesAsync(isReferenceActive: false)).ToList();
            else /* Muestra solo Items y referencias activas */
                references = (await ItemReferenceService.GetReportsReferencesAsync(isItemActive: true, isReferenceActive: true)).ToList();

            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);

            StateHasChanged();
        }
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
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
