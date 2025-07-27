using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.MinimumWarehouseStock.Components
{
    public partial class MinimumWarehouseStockReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected IProviderReferenceService ProviderReferenceService { get; set; }

        [Inject]
        protected IItemService ItemService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public MinimumWarehouseStockFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected List<Warehouse> Warehouses = new();
        protected List<ProviderViewModel> Providers = new();
        protected List<ItemViewModel> AvailableItems = new();
        protected List<ReferenceViewModel> AvailableReferences = new();
        protected bool ValidationError = false;
        protected string ValidationErrorMessage = string.Empty;
        protected short? WarehouseId;
        protected bool OnlyBelowMinimum = true;
        protected int? UmbralMaximoExistencias;
        protected List<int> SelectedProviderIds = new List<int>();
        protected List<int> SelectedItemIds = new List<int>();
        protected List<int> SelectedReferenceIds = new List<int>();
        protected bool ShowReferenceValidationMessage = false;

        // Filtros de texto libre para campos del proveedor
        protected string ProviderItemName = string.Empty;
        protected string ProviderReference = string.Empty;
        protected string ProviderReferenceName = string.Empty;
        protected string ProviderReferenceCode = string.Empty;

        // Filtros de checkbox de 3 estados para Artículos
        protected bool? ItemIsActive = null;
        protected bool? ItemIsDomesticProduct = null;
        protected bool? ItemIsSpecialImport = null;
        protected bool? ItemIsSaleOff = null;
        protected bool? ItemIsCatalogVisible = null;

        // Filtros de checkbox de 3 estados para Referencias
        protected bool? ReferenceIsActive = null;
        protected bool? ReferenceHasAlarmMinimumQuantity = null;
        protected bool? ReferenceIsSoldOut = null;

        // Referencias a los grids
        protected RadzenDropDownDataGrid<List<int>> providersGrid;
        protected RadzenDropDownDataGrid<List<int>> itemsGrid;
        protected RadzenDropDownDataGrid<List<int>> referencesGrid;
        #endregion

        #region ViewModels para el componente
        public class ProviderViewModel
        {
            public int ProviderId { get; set; }
            public string ProviderName { get; set; } = string.Empty;
            public string ProviderCode { get; set; } = string.Empty;
        }

        public class ItemViewModel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public string InternalReference { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }

        public class ReferenceViewModel
        {
            public int ReferenceId { get; set; }
            public string ReferenceName { get; set; } = string.Empty;
            public string ReferenceCode { get; set; } = string.Empty;
            public Item Item { get; set; } = new();
            public bool IsActive { get; set; }
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new MinimumWarehouseStockFilter();
            
            // Cargar datos base
            Warehouses = (await WarehouseService.GetAsync()).ToList();
            await LoadProviders();
            await LoadAvailableItems();
        }

        protected bool FirstRender = true;
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (FirstRender == false) return;
            
            if (Filter != null)
            {
                WarehouseId = Filter.WarehouseId;
                OnlyBelowMinimum = Filter.OnlyBelowMinimum;
                UmbralMaximoExistencias = Filter.UmbralMaximoExistencias;
                
                // Cargar filtros de texto libre
                ProviderItemName = Filter.ProviderItemName ?? string.Empty;
                ProviderReference = Filter.ProviderReference ?? string.Empty;
                ProviderReferenceName = Filter.ProviderReferenceName ?? string.Empty;
                ProviderReferenceCode = Filter.ProviderReferenceCode ?? string.Empty;
                
                // Cargar filtros de checkbox de 3 estados para Artículos
                ItemIsActive = Filter.ItemIsActive;
                ItemIsDomesticProduct = Filter.ItemIsDomesticProduct;
                ItemIsSpecialImport = Filter.ItemIsSpecialImport;
                ItemIsSaleOff = Filter.ItemIsSaleOff;
                ItemIsCatalogVisible = Filter.ItemIsCatalogVisible;
                
                // Cargar filtros de checkbox de 3 estados para Referencias
                ReferenceIsActive = Filter.ReferenceIsActive;
                ReferenceHasAlarmMinimumQuantity = Filter.ReferenceHasAlarmMinimumQuantity;
                ReferenceIsSoldOut = Filter.ReferenceIsSoldOut;
                
                // Cargar selecciones desde strings de IDs
                if (!string.IsNullOrEmpty(Filter.ProviderIds))
                {
                    SelectedProviderIds = Filter.ProviderIds.Split(',').Select(int.Parse).ToList();
                }
                
                if (!string.IsNullOrEmpty(Filter.ItemIds))
                {
                    SelectedItemIds = Filter.ItemIds.Split(',').Select(int.Parse).ToList();
                    await LoadAvailableReferences();
                }
                
                if (!string.IsNullOrEmpty(Filter.ReferenceIds))
                {
                    SelectedReferenceIds = Filter.ReferenceIds.Split(',').Select(int.Parse).ToList();
                }
            }
            
            FirstRender = false;
            StateHasChanged();
        }

        #region Events
        protected async Task OnProvidersChanged(object value)
        {
            SelectedProviderIds = ((IEnumerable<int>)value)?.ToList() ?? new List<int>();
            
            // Limpiar selecciones dependientes
            SelectedItemIds = new List<int>();
            SelectedReferenceIds = new List<int>();
            AvailableReferences.Clear();
            ShowReferenceValidationMessage = false;
            
            // Recargar artículos filtrados por proveedores
            await LoadAvailableItems();
            StateHasChanged();
        }

        protected async Task OnItemsChanged(object value)
        {
            SelectedItemIds = ((IEnumerable<int>)value)?.ToList() ?? new List<int>();
            
            // Limpiar referencias seleccionadas
            SelectedReferenceIds = new List<int>();
            ShowReferenceValidationMessage = false;
            
            // Cargar referencias de los artículos seleccionados
            await LoadAvailableReferences();
            StateHasChanged();
        }

        protected async Task OnReferencesChanged(object value)
        {
            SelectedReferenceIds = ((IEnumerable<int>)value)?.ToList() ?? new List<int>();
            ShowReferenceValidationMessage = false;
        }

        // Métodos para toggle de selección de headers
        protected async Task OnProviderHeaderToggleSelection(bool value)
        {
            if (value)
            {
                // Seleccionar todos
                SelectedProviderIds = Providers.Select(p => p.ProviderId).ToList();
            }
            else
            {
                // Deseleccionar todos
                SelectedProviderIds = new List<int>();
            }
            await OnProvidersChanged(SelectedProviderIds);
        }

        protected async Task OnItemHeaderToggleSelection(bool value)
        {
            if (value)
            {
                // Seleccionar todos
                SelectedItemIds = AvailableItems.Select(i => i.ItemId).ToList();
            }
            else
            {
                // Deseleccionar todos
                SelectedItemIds = new List<int>();
            }
            await OnItemsChanged(SelectedItemIds);
        }

        protected async Task OnReferenceHeaderToggleSelection(bool value)
        {
            if (value)
            {
                // Seleccionar todos
                SelectedReferenceIds = AvailableReferences.Select(r => r.ReferenceId).ToList();
            }
            else
            {
                // Deseleccionar todos
                SelectedReferenceIds = new List<int>();
            }
            await OnReferencesChanged(SelectedReferenceIds);
        }

        protected async Task LoadProviders()
        {
            var (providers, _) = await ProviderService.GetAsync();
            Providers = providers.Select(p => new ProviderViewModel
            {
                ProviderId = p.ProviderId,
                ProviderName = p.ProviderName,
                ProviderCode = p.ProviderCode
            }).ToList();
        }

        protected async Task LoadAvailableItems()
        {
            var items = await ItemService.GetAsync();
            
            if (SelectedProviderIds?.Any() == true)
            {
                // Filtrar artículos por proveedores seleccionados
                var allProviderReferences = new List<ProviderReference>();
                foreach (var providerId in SelectedProviderIds)
                {
                    var providerRefs = await ProviderReferenceService.GetByProviderIdAsync(providerId);
                    allProviderReferences.AddRange(providerRefs);
                }
                
                var itemIds = allProviderReferences.Select(pr => pr.ItemReference.ItemId).Distinct();
                AvailableItems = items.Where(i => itemIds.Contains(i.ItemId) && i.IsActive)
                    .Select(i => new ItemViewModel
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        InternalReference = i.InternalReference,
                        IsActive = i.IsActive
                    }).ToList();
            }
            else
            {
                // Cargar todos los artículos activos
                AvailableItems = items.Where(i => i.IsActive)
                    .Select(i => new ItemViewModel
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        InternalReference = i.InternalReference,
                        IsActive = i.IsActive
                    }).ToList();
            }
        }

        protected async Task LoadAvailableReferences()
        {
            if (SelectedItemIds?.Any() == true)
            {
                var allReferences = new List<ItemReference>();
                foreach (var itemId in SelectedItemIds)
                {
                    var itemRefs = await ItemReferenceService.GetByItemIdAsync(itemId);
                    allReferences.AddRange(itemRefs.Where(r => r.IsActive));
                }
                
                AvailableReferences = allReferences.Select(r => new ReferenceViewModel
                {
                    ReferenceId = r.ReferenceId,
                    ReferenceName = r.ReferenceName,
                    ReferenceCode = r.ReferenceCode,
                    Item = r.Item,
                    IsActive = r.IsActive
                }).ToList();
            }
            else
            {
                AvailableReferences.Clear();
            }
        }

        protected bool HasItemsSelected()
        {
            return SelectedItemIds?.Any() == true;
        }

        protected string GetReferencePlaceholder()
        {
            if (!HasItemsSelected())
            {
                ShowReferenceValidationMessage = true;
                return "Seleccione primero al menos un artículo";
            }
            
            ShowReferenceValidationMessage = false;
            return "Seleccione referencias...";
        }

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationError = false;
                ValidationErrorMessage = string.Empty;

                // Validación obligatoria: Umbral máximo de existencias
                if (!UmbralMaximoExistencias.HasValue)
                {
                    ValidationError = true;
                    ValidationErrorMessage = "El umbral máximo de existencias es obligatorio";
                    return;
                }

                // Configurar filtro básico
                Filter.WarehouseId = WarehouseId;
                Filter.Warehouse = Filter.WarehouseId != null ? Warehouses.FirstOrDefault(s => s.WarehouseId == Filter.WarehouseId.Value) : null;
                Filter.OnlyBelowMinimum = OnlyBelowMinimum;
                Filter.UmbralMaximoExistencias = UmbralMaximoExistencias;
                
                // Configurar filtros de texto libre
                Filter.ProviderItemName = ProviderItemName?.Trim() ?? string.Empty;
                Filter.ProviderReference = ProviderReference?.Trim() ?? string.Empty;
                Filter.ProviderReferenceName = ProviderReferenceName?.Trim() ?? string.Empty;
                Filter.ProviderReferenceCode = ProviderReferenceCode?.Trim() ?? string.Empty;
                
                // Configurar filtros de checkbox de 3 estados para Artículos
                Filter.ItemIsActive = ItemIsActive;
                Filter.ItemIsDomesticProduct = ItemIsDomesticProduct;
                Filter.ItemIsSpecialImport = ItemIsSpecialImport;
                Filter.ItemIsSaleOff = ItemIsSaleOff;
                Filter.ItemIsCatalogVisible = ItemIsCatalogVisible;
                
                // Configurar filtros de checkbox de 3 estados para Referencias
                Filter.ReferenceIsActive = ReferenceIsActive;
                Filter.ReferenceHasAlarmMinimumQuantity = ReferenceHasAlarmMinimumQuantity;
                Filter.ReferenceIsSoldOut = ReferenceIsSoldOut;
                
                // Configurar IDs como strings separados por comas
                Filter.ProviderIds = SelectedProviderIds?.Any() == true ? string.Join(",", SelectedProviderIds) : string.Empty;
                Filter.ItemIds = SelectedItemIds?.Any() == true ? string.Join(",", SelectedItemIds) : string.Empty;
                Filter.ReferenceIds = SelectedReferenceIds?.Any() == true ? string.Join(",", SelectedReferenceIds) : string.Empty;
                
                // Cargar objetos para mostrar en la UI (mantener compatibilidad)
                if (SelectedProviderIds?.Any() == true)
                {
                    var (allProviders, _) = await ProviderService.GetAsync();
                    Filter.SelectedProviders = allProviders.Where(p => SelectedProviderIds.Contains(p.ProviderId)).ToList();
                }
                
                if (SelectedItemIds?.Any() == true)
                {
                    var allItems = await ItemService.GetAsync();
                    Filter.SelectedItems = allItems.Where(i => SelectedItemIds.Contains(i.ItemId)).ToList();
                }
                
                if (SelectedReferenceIds?.Any() == true)
                {
                    var allReferences = new List<ItemReference>();
                    foreach (var itemId in SelectedItemIds ?? new List<int>())
                    {
                        var itemRefs = await ItemReferenceService.GetByItemIdAsync(itemId);
                        allReferences.AddRange(itemRefs);
                    }
                    Filter.SelectedReferences = allReferences.Where(r => SelectedReferenceIds.Contains(r.ReferenceId)).ToList();
                    // Mantener compatibilidad con la propiedad anterior
                    Filter.ItemReferences = Filter.SelectedReferences;
                }
                
                DialogService.Close(Filter);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
                ValidationError = true;
                ValidationErrorMessage = "Ocurrió un error al procesar el filtro";
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

        protected async Task ItemReferenceHandler(List<ItemReference> itemReferences)
        {
            ValidationError = false;
            ValidationErrorMessage = string.Empty;
        }
        #endregion
    }
}