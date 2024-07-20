using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Reservations.Components
{
    public partial class CustomerReservationReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public CustomerReservationFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected RadzenDropDownDataGrid<int?> customerDropdown;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected MultiReferencePicker referencePicker;
        protected List<StatusDocumentType> StatusDocumentTypes = new();
        protected List<Customer> Customers = new();
        protected bool FirstRender = true;
        protected bool ValidationError = false;
        protected bool ValidationCreationDate = false;
        protected bool ValidationReservationDate = false;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            Filter ??= new CustomerReservationFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            var documentType = await DocumentTypeService.FindByCodeAsync("R");
            StatusDocumentTypes = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).ToList();
            Customers = (await CustomerService.GetAsync()).Customers.ToList();
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
        protected async Task FormSubmit()
        {
            try
            {
                ValidationError = false;
                ValidationCreationDate = false;
                ValidationReservationDate = false;
                IsSubmitInProgress = true;

                // Si no se han incluido filtros, mostrar mensaje de error
                if (string.IsNullOrEmpty(Filter.ReservationNumber) &&
                    Filter.CreationDate?.StartDate == null && Filter.CreationDate?.EndDate == null &&
                    Filter.ReservationDate?.StartDate == null && Filter.ReservationDate?.EndDate == null &&
                    Filter.StatusDocumentTypeId == null && Filter.CustomerId == null && SelectedReferences.Any() == false)
                {
                    ValidationError = true;
                    return;
                }

                if ((Filter.CreationDate?.StartDate == null && Filter.CreationDate?.EndDate != null) ||
                    (Filter.CreationDate?.StartDate != null && Filter.CreationDate?.EndDate == null))
                {
                    ValidationCreationDate = true;
                    return;
                }

                if ((Filter.ReservationDate?.StartDate == null && Filter.ReservationDate?.EndDate != null) ||
                    (Filter.ReservationDate?.StartDate != null && Filter.ReservationDate?.EndDate == null))
                {
                    ValidationReservationDate = true;
                    return;
                }

                Filter.ReservationNumber = string.IsNullOrEmpty(Filter.ReservationNumber) ? null : Filter.ReservationNumber;
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
