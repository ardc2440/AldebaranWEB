using Aldebaran.Application.Services;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ReportPages.Automatic_Purchase_Order_Assigment.Components
{
    public partial class AutomaticAssigmentReporFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public ViewModel.AutomaticAssigmentFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected RadzenDropDownDataGrid<int?> providersDropdown;
        protected RadzenDropDownDataGrid<int?> customersDropdown;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected MultiReferencePicker referencePicker;
        protected List<Provider> Providers = new();
        protected List<Customer> Customers = new();
        protected List<StatusDocumentType> StatusDocumentTypes = new();

        protected bool ValidationError = false;
        protected bool ValidationConfirmedDate = false;
        protected bool ValidationReceiptDate = false;
        protected bool ValidationEstimatedDeliveryDate = false;
        protected bool ValidationOrderDate = false;
        protected bool FirstRender = true;
        protected int countProviders = 0;
        protected int countCustomers = 0;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            Filter ??= new ViewModel.AutomaticAssigmentFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            var documentType = await DocumentTypeService.FindByCodeAsync("P");
            StatusDocumentTypes = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).ToList();

            var (providers, _count) = await ProviderService.GetAsync(0, 5);
            Providers = providers.ToList();
            countProviders = _count;

            var (customers, _countCustomers) = await CustomerService.GetAsync(0, 5);
            Customers = customers.ToList();
            countCustomers = _countCustomers;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            
            if (FirstRender == false) return;
            
            if (Filter?.ItemReferences?.Any() == true)
            {
                referencePicker.SetSelectedItemReferences(Filter.ItemReferences.Select(s => s.ReferenceId).ToList());
            }
            
            if (Filter.ProviderId != null)
            {
                var provider = Providers.Single(s => s.ProviderId == Filter.ProviderId.Value);
                await providersDropdown.DataGrid.SelectRow(provider, false);
            }            
            
            FirstRender = false;
            StateHasChanged();
        }
        #endregion

        #region Events
        protected async Task LoadProviderData(LoadDataArgs args)
        {
            await Task.Yield();
            var (providers, _count) = string.IsNullOrEmpty(args.Filter) ? await ProviderService.GetAsync(args.Skip.Value, args.Top.Value) : await ProviderService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            Providers = providers.ToList();
            
            countProviders = _count;
        }

        protected async Task LoadCustomerData(LoadDataArgs args)
        {
            await Task.Yield();
            var (customers, _count) = string.IsNullOrEmpty(args.Filter) ? await CustomerService.GetAsync(args.Skip.Value, args.Top.Value) : await CustomerService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            Customers = customers.ToList();

            countCustomers = _count;
        }

        protected async Task FormSubmit()
        {
            try
            {
                ValidationError = false;
                ValidationConfirmedDate = false;
                ValidationReceiptDate = false;
                ValidationEstimatedDeliveryDate = false;
                ValidationOrderDate = false;
                IsSubmitInProgress = true;

                // Si no se han incluido filtros, mostrar mensaje de error
                if (string.IsNullOrEmpty(Filter.PurchaseOrderNumber) &&
                    string.IsNullOrEmpty(Filter.CustomerOrderNumber) &&
                    string.IsNullOrEmpty(Filter.ProformaNumber) &&
                    string.IsNullOrEmpty(Filter.ImportNumber) &&
                    Filter.ConfirmedDate?.StartDate == null && Filter.ConfirmedDate?.EndDate == null &&
                    Filter.ReceiptDate?.StartDate == null && Filter.ReceiptDate?.EndDate == null &&
                    Filter.EstimatedDeliveryDate?.StartDate == null && Filter.EstimatedDeliveryDate?.EndDate == null &&
                    Filter.OrderDate?.StartDate == null && Filter.OrderDate?.EndDate == null &&
                    Filter.ProviderId == null && 
                    Filter.CustomerId == null && 
                    SelectedReferences.Any() == false && 
                    Filter.StatusDocumentTypeId == null)
                {
                    ValidationError = true;
                    return;
                }

                if ((Filter.ConfirmedDate?.StartDate == null && Filter.ConfirmedDate?.EndDate != null) ||
                    (Filter.ConfirmedDate?.StartDate != null && Filter.ConfirmedDate?.EndDate == null))
                {
                    ValidationConfirmedDate = true;
                    return;
                }

                if ((Filter.ReceiptDate?.StartDate == null && Filter.ReceiptDate?.EndDate != null) ||
                    (Filter.ReceiptDate?.StartDate != null && Filter.ReceiptDate?.EndDate == null))
                {
                    ValidationReceiptDate = true;
                    return;
                }
                if ((Filter.EstimatedDeliveryDate?.StartDate == null && Filter.EstimatedDeliveryDate?.EndDate != null) ||
                    (Filter.EstimatedDeliveryDate?.StartDate != null && Filter.EstimatedDeliveryDate?.EndDate == null))
                {
                    ValidationEstimatedDeliveryDate = true;
                    return;
                }
                if ((Filter.OrderDate?.StartDate == null && Filter.OrderDate?.EndDate != null) ||
                    (Filter.OrderDate?.StartDate != null && Filter.OrderDate?.EndDate == null))
                {
                    ValidationOrderDate = true;
                    return;
                }
                Filter.PurchaseOrderNumber = string.IsNullOrEmpty(Filter.PurchaseOrderNumber) ? null : Filter.PurchaseOrderNumber;
                Filter.ImportNumber = string.IsNullOrEmpty(Filter.ImportNumber) ? null : Filter.ImportNumber;
                Filter.ProformaNumber = string.IsNullOrEmpty(Filter.ProformaNumber) ? null : Filter.ProformaNumber;
                Filter.Provider = Filter.ProviderId != null ? Providers.FirstOrDefault(s => s.ProviderId == Filter.ProviderId.Value) : null;
                Filter.Customer = Filter.CustomerId != null ? Customers.FirstOrDefault(s => s.CustomerId == Filter.CustomerId.Value) : null;
                Filter.ItemReferences = SelectedReferences;
                Filter.StatusDocumentType = Filter.StatusDocumentTypeId != null ? StatusDocumentTypes.FirstOrDefault(s => s.StatusDocumentTypeId == Filter.StatusDocumentTypeId.Value) : null;
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
