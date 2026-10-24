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
        /// <summary>Artículo activo (Sí/No/Todos): acota las referencias disponibles y limpia la selección.</summary>
        protected async Task OnItemActiveChanged(bool? value)
        {
            Filter.IsItemActive = value;
            await ResetSelectionAsync();
        }

        /// <summary>Referencia activa (Sí/No/Todos): acota las referencias disponibles y limpia la selección.</summary>
        protected async Task OnReferenceActiveChanged(bool? value)
        {
            Filter.IsReferenceActive = value;
            await ResetSelectionAsync();
        }

        private async Task ResetSelectionAsync()
        {
            Filter?.ItemReferences.Clear();
            await AvailableReferenceFilter();
        }

        /// <summary>Precio bomba (casilla de tres estados): acota las referencias disponibles y limpia la selección.</summary>
        protected async Task OnBombPriceChanged(bool? value)
        {
            Filter.BombPrice = value;
            await ResetSelectionAsync();
        }

        protected async Task AvailableReferenceFilter()
        {
            Filter ??= new ReferenceMovementFilter();

            var references = new List<ItemReference>();

            if (Filter?.ItemReferences?.Any() == true && Filter.AllMovementCheckVisible) /* Muestra solo la referencia seleccionada en las alarmas*/
                references.Add(await ItemReferenceService.FindAsync(Filter.ItemReferences.FirstOrDefault().ReferenceId));
            else /* Referencias según las condiciones Artículo activo / Referencia activa (null = Todos) */
                references = (await ItemReferenceService.GetReportsReferencesAsync(isReferenceActive: Filter.IsReferenceActive, isItemActive: Filter.IsItemActive)).ToList();

            AvailableItemReferencesForSelection = references
                .Where(r => Filter.BombPrice == null || r.Item?.IsBombPrice == Filter.BombPrice)
                .ToList();
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
