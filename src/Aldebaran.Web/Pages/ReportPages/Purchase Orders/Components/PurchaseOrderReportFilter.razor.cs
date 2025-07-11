﻿using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Purchase_Orders.Components
{
    public partial class PurchaseOrderReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected IForwarderService ForwarderService { get; set; }

        [Inject]
        protected IForwarderAgentService ForwarderAgentService { get; set; }

        [Inject]
        protected IShipmentMethodService ShipmentMethodService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IShipmentForwarderAgentMethodService ShipmentForwarderAgentMethodService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public Purchase_Orders.ViewModel.PurchaseOrderFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected RadzenDropDownDataGrid<int?> providersDropdown;
        protected RadzenDropDownDataGrid<int?> forwardersDropdown;
        protected RadzenDropDownDataGrid<int?> forwarderAgentsDropdown;
        protected RadzenDropDown<short?> shipmentMethodsDropdown;
        protected List<ItemReference> SelectedReferences = new();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        protected MultiReferencePicker referencePicker;
        protected List<Provider> Providers = new();
        protected List<Forwarder> Forwarders = new();
        protected List<ForwarderAgent> ForwarderAgents = new();
        protected List<ShipmentMethod> ShipmentMethods = new();

        protected List<Warehouse> Warehouses = new();
        protected List<StatusDocumentType> StatusDocumentTypes = new();
        protected bool ValidationError = false;
        protected bool ValidationCreationDate = false;
        protected bool ValidationRequestDate = false;
        protected bool ValidationExpectedReceiptDate = false;                

        protected int? ForwarderId;
        protected int? ForwarderAgentId;
        protected short? ShipmentMethodId;
        protected bool FirstRender = true;

        protected int count = 0;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            Filter ??= new Purchase_Orders.ViewModel.PurchaseOrderFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);            
            Forwarders = (await ForwarderService.GetAsync()).ToList();
            Warehouses = (await WarehouseService.GetAsync()).ToList();
            var documentType = await DocumentTypeService.FindByCodeAsync("O");
            StatusDocumentTypes = (await StatusDocumentTypeService.GetByDocumentTypeIdAsync(documentType.DocumentTypeId)).ToList();

            var (providers, _count) = await ProviderService.GetAsync(0, 5);
            Providers = providers.ToList();
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
            if (Filter.ProviderId != null)
            {
                var provider = Providers.Single(s => s.ProviderId == Filter.ProviderId.Value);
                await providersDropdown.DataGrid.SelectRow(provider, false);
            }
            if (Filter.ForwarderId != null)
            {
                ForwarderId = Filter.ForwarderId;
                var forwarder = Forwarders.Single(s => s.ForwarderId == Filter.ForwarderId.Value);
                await forwardersDropdown.DataGrid.SelectRow(forwarder, false);
                await OnForwarderChange(Filter.ForwarderId);
            }
            if (Filter.ForwarderAgentId != null && ForwarderAgents?.Any() == true)
            {
                ForwarderAgentId = Filter.ForwarderAgentId;
                var forwarderAgent = ForwarderAgents.Single(s => s.ForwarderAgentId == Filter.ForwarderAgentId.Value);
                await forwarderAgentsDropdown.DataGrid.SelectRow(forwarderAgent, true);
                await OnForwarderAgentChange(Filter.ForwarderAgentId);
            }
            if (Filter.ShipmentMethodId != null && ShipmentMethods?.Any() == true)
            {
                ShipmentMethodId = Filter.ShipmentMethodId;
                var shipmentMethod = ShipmentMethods.Single(s => s.ShipmentMethodId == Filter.ShipmentMethodId.Value);
                await shipmentMethodsDropdown.SelectItem(shipmentMethod, false);
            }
            
            FirstRender = false;
            StateHasChanged();
        }
        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            await Task.Yield();
            var (providers, _count) = string.IsNullOrEmpty(args.Filter) ? await ProviderService.GetAsync(args.Skip.Value, args.Top.Value) : await ProviderService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            Providers = providers.ToList();
            count = _count;
        }

        protected async Task FormSubmit()
        {
            try
            {
                ValidationError = false;
                ValidationCreationDate = false;
                ValidationRequestDate = false; 
                ValidationExpectedReceiptDate = false;
                IsSubmitInProgress = true;

                // Si no se han incluido filtros, mostrar mensaje de error
                if (string.IsNullOrEmpty(Filter.OrderNumber) &&
                    Filter.CreationDate?.StartDate == null && Filter.CreationDate?.EndDate == null &&
                    Filter.RequestDate?.StartDate == null && Filter.RequestDate?.EndDate == null &&
                    Filter.ExpectedReceiptDate?.StartDate == null && Filter.ExpectedReceiptDate?.EndDate == null &&
                    string.IsNullOrEmpty(Filter.ImportNumber) && string.IsNullOrEmpty(Filter.EmbarkationPort) && string.IsNullOrEmpty(Filter.ProformaNumber) &&
                    Filter.ProviderId == null && ForwarderId == null && ForwarderAgentId == null && ShipmentMethodId == null &&
                    Filter.WarehouseId == null && SelectedReferences.Any() == false && Filter.StatusDocumentId == null)
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

                if ((Filter.RequestDate?.StartDate == null && Filter.RequestDate?.EndDate != null) ||
                    (Filter.RequestDate?.StartDate != null && Filter.RequestDate?.EndDate == null))
                {
                    ValidationRequestDate = true;
                    return;
                }
                if ((Filter.ExpectedReceiptDate?.StartDate == null && Filter.ExpectedReceiptDate?.EndDate != null) ||
                    (Filter.ExpectedReceiptDate?.StartDate != null && Filter.ExpectedReceiptDate?.EndDate == null))
                {
                    ValidationExpectedReceiptDate = true;
                    return;
                }
                               
                Filter.OrderNumber = string.IsNullOrEmpty(Filter.OrderNumber) ? null : Filter.OrderNumber;
                Filter.ImportNumber = string.IsNullOrEmpty(Filter.ImportNumber) ? null : Filter.ImportNumber;
                Filter.EmbarkationPort = string.IsNullOrEmpty(Filter.EmbarkationPort) ? null : Filter.EmbarkationPort;
                Filter.ProformaNumber = string.IsNullOrEmpty(Filter.ProformaNumber) ? null : Filter.ProformaNumber;
                Filter.Provider = Filter.ProviderId != null ? Providers.FirstOrDefault(s => s.ProviderId == Filter.ProviderId.Value) : null;
                Filter.ForwarderId = ForwarderId;
                Filter.Forwarder = Filter.ForwarderId != null ? Forwarders.FirstOrDefault(s => s.ForwarderId == Filter.ForwarderId.Value) : null;
                Filter.ForwarderAgentId = ForwarderAgentId;
                Filter.ForwarderAgent = Filter.ForwarderAgentId != null ? ForwarderAgents.FirstOrDefault(s => s.ForwarderAgentId == Filter.ForwarderAgentId.Value) : null;
                Filter.ShipmentMethodId = ShipmentMethodId;
                Filter.ShipmentMethod = Filter.ShipmentMethodId != null ? ShipmentMethods.FirstOrDefault(s => s.ShipmentMethodId == Filter.ShipmentMethodId.Value) : null;
                Filter.Warehouse = Filter.WarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.WarehouseId.Value) : null;
                Filter.ItemReferences = SelectedReferences;
                Filter.StatusDocumentType = Filter.StatusDocumentId != null ? StatusDocumentTypes.FirstOrDefault(s => s.StatusDocumentTypeId == Filter.StatusDocumentId.Value) : null;
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
        protected async Task OnForwarderChange(object id)
        {
            int? forwarderId = (int?)id;
            ForwarderAgentId = null;
            ForwarderAgents = forwarderId == null ? new() : (await ForwarderAgentService.GetByForwarderIdAsync((int)forwarderId)).ToList();
            await OnForwarderAgentChange(null);
        }
        protected async Task OnForwarderAgentChange(object id)
        {
            int? forwarderAgentId = (int?)id;
            ShipmentMethodId = null;
            if (forwarderAgentId == null)
            {
                ShipmentMethods = new();
                return;
            }
            var shipmentForwarderAgentMethods = await ShipmentForwarderAgentMethodService.GetByForwarderAgentIdAsync(forwarderAgentId.Value);
            ShipmentMethods = shipmentForwarderAgentMethods?.Select(s => s.ShipmentMethod).ToList() ?? new();
        }

        #endregion
    }
}
