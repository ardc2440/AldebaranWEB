using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.Componentes
{
    public partial class InventoryAdjustmentsReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IAdjustmentReasonService AdjustmentReasonService { get; set; }

        [Inject]
        protected IAdjustmentTypeService AdjustmentTypeService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public InventoryAdjustmentsFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected List<Employee> Employees = new();
        protected List<AdjustmentReason> AdjustmentsReasons = new();
        protected List<AdjustmentType> AdjustmentsTypes = new();
        protected MultiReferencePicker referencePicker;
        protected bool ValidationError = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new InventoryAdjustmentsFilter();
            var references = (await ItemReferenceService.GetAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            Employees = (await EmployeeService.GetAsync()).ToList();
            AdjustmentsReasons = (await AdjustmentReasonService.GetAsync()).ToList();
            AdjustmentsTypes = (await AdjustmentTypeService.GetAsync()).ToList();
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
        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                // Si no se han incluido filtros, mostrar mensaje de error
                if (Filter.AdjustmentId == null && Filter.CreationDate == null &&
                    Filter.AdjustmentDate == null && Filter.AdjustmentTypeId == null && Filter.AdjustmentReasonId == null &&
                    Filter.EmployeeId == null && SelectedReferences.Any() == false)
                {
                    ValidationError = true;
                    return;
                }

                Filter.AdjustmentType = Filter.AdjustmentTypeId != null ? AdjustmentsTypes.Single(s => s.AdjustmentTypeId == Filter.AdjustmentTypeId.Value) : null;
                Filter.AdjustmentReason = Filter.AdjustmentReasonId != null ? AdjustmentsReasons.Single(s => s.AdjustmentReasonId == Filter.AdjustmentReasonId.Value) : null;
                Filter.Employee = Filter.EmployeeId != null ? Employees.Single(s => s.EmployeeId == Filter.EmployeeId.Value) : null;
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
