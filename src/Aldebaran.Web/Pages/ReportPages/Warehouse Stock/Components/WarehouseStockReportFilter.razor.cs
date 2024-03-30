using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.Components
{
    public partial class WarehouseStockReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IWarehouseService WarehouseService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public WarehouseStockFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected List<Warehouse> Warehouses = new();
        protected MultiReferencePicker referencePicker;
        protected bool ValidationError = false;
        protected short? WarehouseId;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new WarehouseStockFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            Warehouses = (await WarehouseService.GetAsync()).ToList();
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
            WarehouseId = Filter?.WarehouseId;
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
                if (WarehouseId == null && SelectedReferences.Any() == false)
                {
                    ValidationError = true;
                    return;
                }
                Filter.WarehouseId = WarehouseId;
                Filter.Warehouse = Filter.WarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.WarehouseId.Value) : null;
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
