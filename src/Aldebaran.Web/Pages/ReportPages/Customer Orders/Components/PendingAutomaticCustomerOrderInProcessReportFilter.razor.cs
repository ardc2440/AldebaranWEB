using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders.Components
{
    public partial class PendingAutomaticCustomerOrderInProcessReportFilter
    {
        #region Injections
        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        
        #endregion

        #region Parameters
        [Parameter]
        public PendingAutomaticCustomerOrderInProcessFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected RadzenDropDownDataGrid<int?> customerDropdown;
        protected List<StatusDocumentType> StatusDocumentTypes = new();
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected List<Customer> Customers = new();
        protected MultiReferencePicker referencePicker;
        protected bool FirstRender = true;
        protected bool ValidationError = false;
        protected bool ValidationOrderDate = false;
        protected bool ValidationProcessDate = false;

        protected int count = 0;
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            Filter ??= new PendingAutomaticCustomerOrderInProcessFilter();

            // Get status document types for "Process" documents (T for Traslado/Process)
            var documentType = await DocumentTypeService.FindByCodeAsync("P");
            StatusDocumentTypes = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).Where(w => w.StatusOrder >= 1 && w.StatusOrder <= 3).ToList();

            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            var (customers, _count) = await CustomerService.GetAsync(0, 5);
            Customers = customers.ToList();
            count = _count;
        }

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
        #endregion

        #region Events

        protected async Task LoadData(LoadDataArgs args)
        {
            await Task.Yield();
            var (customers, _count) = string.IsNullOrEmpty(args.Filter) ? await CustomerService.GetAsync(args.Skip.Value, args.Top.Value) : await CustomerService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            Customers = customers.ToList();
            count = _count;
        }

        protected async Task FormSubmit()
        {
            try
            {
                ValidationError = false;
                ValidationOrderDate = false;
                ValidationProcessDate = false;

                IsSubmitInProgress = true;

                if ((Filter.OrderDate?.StartDate == null && Filter.OrderDate?.EndDate != null) ||
                    (Filter.OrderDate?.StartDate != null && Filter.OrderDate?.EndDate == null))
                {
                    ValidationOrderDate = true;
                    return;
                }

                if ((Filter.ProcessDate?.StartDate == null && Filter.ProcessDate?.EndDate != null) ||
                    (Filter.ProcessDate?.StartDate != null && Filter.ProcessDate?.EndDate == null))
                {
                    ValidationProcessDate = true;
                    return;
                }

                Filter.OrderNumber = string.IsNullOrEmpty(Filter.OrderNumber) ? null : Filter.OrderNumber;
                Filter.StatusDocumentType = Filter.StatusDocumentTypeId != null ? StatusDocumentTypes.FirstOrDefault(s => s.StatusDocumentTypeId == Filter.StatusDocumentTypeId.Value) : null;
                Filter.Customer = Filter.CustomerId != null ? Customers.FirstOrDefault(s => s.CustomerId == Filter.CustomerId.Value) : null;
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