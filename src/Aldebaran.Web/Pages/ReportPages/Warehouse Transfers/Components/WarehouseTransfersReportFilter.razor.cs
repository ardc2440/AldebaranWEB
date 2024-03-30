using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.Components
{
    public partial class WarehouseTransfersReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public WarehouseTransfersFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected List<Warehouse> Warehouses = new();
        protected MultiReferencePicker referencePicker;
        protected bool ValidationError = false;
        protected short? SourceWarehouseId;
        protected short? TargetWarehouseId;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new WarehouseTransfersFilter();
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
            SourceWarehouseId = Filter?.SourceWarehouseId;
            TargetWarehouseId = Filter?.TargetWarehouseId;
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
                if (string.IsNullOrEmpty(Filter.NationalizationNumber) && Filter.AdjustmentDate == null &&
                    SourceWarehouseId == null && TargetWarehouseId == null && SelectedReferences.Any() == false)
                {
                    ValidationError = true;
                    return;
                }
                Filter.TargetWarehouseId = TargetWarehouseId;
                Filter.TargetWarehouse = Filter.TargetWarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.TargetWarehouseId.Value) : null;
                Filter.SourceWarehouseId = SourceWarehouseId;
                Filter.SourceWarehouse = Filter.SourceWarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.SourceWarehouseId.Value) : null;
                Filter.NationalizationNumber = string.IsNullOrEmpty(Filter.NationalizationNumber) ? null : Filter.NationalizationNumber;
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
