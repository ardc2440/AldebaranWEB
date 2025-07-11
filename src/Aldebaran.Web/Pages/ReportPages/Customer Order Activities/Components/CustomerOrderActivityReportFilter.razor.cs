﻿using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.Components
{
    public partial class CustomerOrderActivityReportFilter
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
        public CustomerOrderActivityFilter Filter { get; set; } = new();
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
        protected bool ValidationError = false;
        protected bool ValidationCreationDate = false;
        protected bool ValidationOrderDate = false;
        protected bool ValidationEstimatedDeliveryDate = false;

        protected int count = 0;
        #endregion

        #region Ovverride
        protected override async Task OnInitializedAsync()
        {
            Filter ??= new CustomerOrderActivityFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
            var documentType = await DocumentTypeService.FindByCodeAsync("P");
            StatusDocumentTypes = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).ToList();
            var (customers, _count) = await CustomerService.GetAsync(0, 5);
            Customers = customers.ToList();
            count = _count;
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
                ValidationCreationDate = false;
                ValidationOrderDate = false;
                ValidationEstimatedDeliveryDate = false;

                IsSubmitInProgress = true;
                // Si no se han incluido filtros, mostrar mensaje de error
                if (Filter.CreationDate?.StartDate == null && Filter.CreationDate?.EndDate == null &&
                    Filter.OrderDate?.StartDate == null && Filter.OrderDate?.EndDate == null &&
                    Filter.EstimatedDeliveryDate?.StartDate == null && Filter.EstimatedDeliveryDate?.EndDate == null &&
                    string.IsNullOrEmpty(Filter.OrderNumber) &&
                    Filter.StatusDocumentTypeId == null &&
                    Filter.CustomerId == null &&
                    SelectedReferences.Any() == false)
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

                if ((Filter.OrderDate?.StartDate == null && Filter.OrderDate?.EndDate != null) ||
                    (Filter.OrderDate?.StartDate != null && Filter.OrderDate?.EndDate == null))
                {
                    ValidationOrderDate = true;
                    return;
                }

                if ((Filter.EstimatedDeliveryDate?.StartDate == null && Filter.EstimatedDeliveryDate?.EndDate != null) ||
                    (Filter.EstimatedDeliveryDate?.StartDate != null && Filter.EstimatedDeliveryDate?.EndDate == null))
                {
                    ValidationEstimatedDeliveryDate = true;
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
