﻿using Aldebaran.Application.Services;
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

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }
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
        protected List<StatusDocumentType> StatusDocumentTypes = new();
        protected MultiReferencePicker referencePicker;
        protected bool ValidationError = false;
        protected bool ValidationAdjustmentDate = false;
        protected short? SourceWarehouseId;
        protected short? TargetWarehouseId;
        protected short? StatusDocumentTypeId;
        protected bool FirstRender = true;
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            Filter ??= new WarehouseTransfersFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            Warehouses = (await WarehouseService.GetAsync()).ToList();
            var documentType = await DocumentTypeService.FindByCodeAsync("B");
            StatusDocumentTypes = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).ToList();
        }
        
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
            StatusDocumentTypeId = Filter?.StatusDocumentTypeId;
            FirstRender = false;
            StateHasChanged();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                ValidationError = false;
                ValidationAdjustmentDate = false;
                IsSubmitInProgress = true;
                // Si no se han incluido filtros, mostrar mensaje de error
                if (string.IsNullOrEmpty(Filter.NationalizationNumber)
                    && Filter.AdjustmentDate?.StartDate == null && Filter.AdjustmentDate?.EndDate == null &&
                    SourceWarehouseId == null && TargetWarehouseId == null &&
                    StatusDocumentTypeId == null && SelectedReferences.Any() == false)
                {
                    ValidationError = true;
                    return;
                }

                if ((Filter.AdjustmentDate?.StartDate == null && Filter.AdjustmentDate?.EndDate != null) ||
                    (Filter.AdjustmentDate?.StartDate != null && Filter.AdjustmentDate?.EndDate == null))
                {
                    ValidationAdjustmentDate = true;
                    return;
                }
                Filter.TargetWarehouseId = TargetWarehouseId;
                Filter.TargetWarehouse = Filter.TargetWarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.TargetWarehouseId.Value) : null;
                Filter.SourceWarehouseId = SourceWarehouseId;
                Filter.SourceWarehouse = Filter.SourceWarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.SourceWarehouseId.Value) : null;
                Filter.StatusDocumentTypeId = StatusDocumentTypeId;
                Filter.StatusDocumentType = Filter.StatusDocumentTypeId != null ? StatusDocumentTypes.FirstOrDefault(s => s.StatusDocumentTypeId == Filter.StatusDocumentTypeId.Value) : null;
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
