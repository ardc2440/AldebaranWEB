using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Pages.ReportPages.Item_References.Components;
using Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Provider_References.Components
{
    /// <summary>
    /// Diálogo de filtros del reporte de Referencias del Proveedor.
    /// Proveedor, jerarquía Línea -> Artículo -> Referencia (sin nivel obligatorio), condiciones de Artículo y
    /// Referencia Sí/No/Todos (mismas reglas del reporte de Artículos y Referencias, sin cantidades mínimas)
    /// y la opción de presentación "Mostrar inventario y bodegas". Las condiciones acotan el catálogo del selector
    /// conservando la selección válida. Devuelve un <see cref="ProviderReferencesFilter"/> o null si se cancela.
    /// </summary>
    public partial class ProviderReferencesReportFilter
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IProviderService ProviderService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        /// <summary>Filtro vigente del reporte, para restaurar proveedor, condiciones y selección al reabrir el diálogo.</summary>
        [Parameter]
        public ProviderReferencesFilter Filter { get; set; }
        #endregion

        #region Variables
        protected HierarchicalReferencePicker referencePicker;
        protected RadzenDropDownDataGrid<int?> providersDropdown;
        protected IEnumerable<Provider> Providers = Array.Empty<Provider>();
        protected int count = 0;
        protected int? ProviderId;
        protected bool IsLoadingReferences = true;
        protected bool IsSubmitInProgress;
        protected bool ValidationError;

        /// <summary>Copia de trabajo; el filtro vigente no se altera si se cancela.</summary>
        protected ProviderReferenceReportFilter Criteria = new();
        protected IReadOnlyList<ConditionOption> ItemConditions = Array.Empty<ConditionOption>();
        protected IReadOnlyList<ConditionOption> ReferenceConditions = Array.Empty<ConditionOption>();

        private List<ItemReference> _catalog = new();
        #endregion

        #region Overrides
        protected override void OnParametersSet()
        {
            var current = Filter?.Criteria ?? new ProviderReferenceReportFilter();
            Criteria = CopyConditions(current, new ProviderReferenceReportFilter());
            ProviderId = current.ProviderId;
            BuildConditionOptions();
        }

        protected override async Task OnInitializedAsync()
        {
            var (providers, total) = await ProviderService.GetAsync(0, 5);
            Providers = IncludeCurrentProvider(providers.ToList());
            count = total;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;
            await LoadPickerAsync();
        }
        #endregion

        #region Proveedor
        protected async Task LoadData(LoadDataArgs args)
        {
            var (providers, total) = string.IsNullOrEmpty(args.Filter)
                ? await ProviderService.GetAsync(args.Skip.Value, args.Top.Value)
                : await ProviderService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            Providers = IncludeCurrentProvider(providers.ToList());
            count = total;
        }

        /// <summary>El proveedor ya elegido debe estar en la lista para que el combo muestre su nombre al reabrir.</summary>
        private List<Provider> IncludeCurrentProvider(List<Provider> providers)
        {
            var current = Filter?.Provider;
            if (current != null && providers.All(p => p.ProviderId != current.ProviderId))
                providers.Insert(0, current);
            return providers;
        }

        private async Task<Provider> GetSelectedProviderAsync()
        {
            if (ProviderId == null)
                return null;
            return Providers.FirstOrDefault(p => p.ProviderId == ProviderId.Value) ?? await ProviderService.FindAsync(ProviderId.Value);
        }
        #endregion

        #region Condiciones
        private void BuildConditionOptions()
        {
            ItemConditions = new List<ConditionOption>
            {
                new("Activo", () => Criteria.IsItemActive, v => Criteria.IsItemActive = v),
                new("Visible en página", () => Criteria.IsCatalogVisible, v => Criteria.IsCatalogVisible = v),
                new("Producto nacional", () => Criteria.IsDomesticProduct, v => Criteria.IsDomesticProduct = v),
                new("Importación especial", () => Criteria.IsSpecialImport, v => Criteria.IsSpecialImport = v),
                new("En oferta", () => Criteria.IsSaleOff, v => Criteria.IsSaleOff = v),
                new("Precio bomba", () => Criteria.BombPrice, v => Criteria.BombPrice = v)
            };
            ReferenceConditions = new List<ConditionOption>
            {
                new("Activa", () => Criteria.IsReferenceActive, v => Criteria.IsReferenceActive = v),
                new("Agotada", () => Criteria.IsSoldOut, v => Criteria.IsSoldOut = v)
            };
        }

        /// <summary>Aplica la condición (Sí/No/Todos) y acota el catálogo del selector conservando la selección válida.</summary>
        protected void OnConditionChanged(ConditionOption option, bool? value)
        {
            option.Set(value);
            ValidationError = false;
            ApplyCatalog(referencePicker.GetSelectedLineIds(), referencePicker.GetSelectedItemIds(), referencePicker.GetSelectedReferenceIds());
        }

        private static ProviderReferenceReportFilter CopyConditions(ProviderReferenceReportFilter source, ProviderReferenceReportFilter target)
        {
            target.IsItemActive = source.IsItemActive;
            target.IsCatalogVisible = source.IsCatalogVisible;
            target.IsDomesticProduct = source.IsDomesticProduct;
            target.IsSpecialImport = source.IsSpecialImport;
            target.IsSaleOff = source.IsSaleOff;
            target.BombPrice = source.BombPrice;
            target.IsReferenceActive = source.IsReferenceActive;
            target.IsSoldOut = source.IsSoldOut;
            target.IncludeInventory = source.IncludeInventory;
            return target;
        }

        /// <summary>
        /// Mismas reglas en memoria que el reporte de Artículos y Referencias (se reutiliza ItemReferencesReportSwitchRules
        /// sin modificarlo); las condiciones de cantidades mínimas no aplican a este reporte y quedan en Todos.
        /// </summary>
        private ItemReferencesReportFilter ToCatalogRules() => new()
        {
            IsItemActive = Criteria.IsItemActive,
            IsCatalogVisible = Criteria.IsCatalogVisible,
            IsDomesticProduct = Criteria.IsDomesticProduct,
            IsSpecialImport = Criteria.IsSpecialImport,
            IsSaleOff = Criteria.IsSaleOff,
            BombPrice = Criteria.BombPrice,
            IsReferenceActive = Criteria.IsReferenceActive,
            IsSoldOut = Criteria.IsSoldOut
        };

        /// <summary>Condición de tres estados: null = Todos, true = Sí, false = No.</summary>
        protected sealed record ConditionOption(string Label, Func<bool?> Get, Action<bool?> Set);
        #endregion

        #region Picker
        /// <summary>Carga el catálogo una sola vez y lo entrega al selector con la selección vigente.</summary>
        private async Task LoadPickerAsync()
        {
            _catalog = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            var current = Filter?.Criteria ?? new ProviderReferenceReportFilter();
            ApplyCatalog(current.LineIds, current.ItemIds, current.ReferenceIds);
            IsLoadingReferences = false;
            StateHasChanged();
        }

        private void ApplyCatalog(IEnumerable<short> lineIds, IEnumerable<int> itemIds, IEnumerable<int> referenceIds)
        {
            referencePicker.SetAvailableItemReferencesForSelection(ItemReferencesReportSwitchRules.Apply(_catalog, ToCatalogRules()).ToList());
            referencePicker.RestoreSelection(lineIds, itemIds, referenceIds);
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                var criteria = BuildCriteria();
                if (!HasAnyFilter(criteria))
                {
                    ValidationError = true;
                    return;
                }
                var provider = await GetSelectedProviderAsync();
                DialogService.Close(new ProviderReferencesFilter(criteria, provider, BuildSummary(criteria)));
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected void CancelButtonClick() => DialogService.Close(null);

        private ProviderReferenceReportFilter BuildCriteria()
        {
            var criteria = CopyConditions(Criteria, new ProviderReferenceReportFilter());
            criteria.ProviderId = ProviderId;
            criteria.LineIds = referencePicker.GetSelectedLineIds();
            criteria.ItemIds = referencePicker.GetSelectedItemIds();
            criteria.ReferenceIds = referencePicker.GetSelectedReferenceIds();
            return criteria;
        }

        /// <summary>Al menos un filtro: proveedor, algún nivel de la jerarquía o una condición en Sí/No. "Mostrar inventario" no es filtro.</summary>
        internal static bool HasAnyFilter(ProviderReferenceReportFilter c) =>
            c.ProviderId != null || c.LineIds.Any() || c.ItemIds.Any() || c.ReferenceIds.Any()
            || new[] { c.IsItemActive, c.IsCatalogVisible, c.IsDomesticProduct, c.IsSpecialImport, c.IsSaleOff, c.BombPrice, c.IsReferenceActive, c.IsSoldOut }.Any(v => v.HasValue);

        /// <summary>Resumen legible: nivel más específico de la jerarquía, condiciones en Sí/No y la presentación sin inventario.</summary>
        private ItemReferencesReportFilterSummary BuildSummary(ProviderReferenceReportFilter criteria)
        {
            var hierarchy = new ItemReferencesReportFilter { LineIds = criteria.LineIds, ItemIds = criteria.ItemIds, ReferenceIds = criteria.ReferenceIds };
            var conditions = ItemConditions.Concat(ReferenceConditions)
                                           .Where(o => o.Get().HasValue)
                                           .Select(o => $"{o.Label}: {(o.Get()!.Value ? "Sí" : "No")}")
                                           .ToList();
            if (!criteria.IncludeInventory)
                conditions.Add("Mostrar inventario y bodegas: No");
            return ItemReferencesReportFilterSummaryBuilder.Build(hierarchy, _catalog, conditions);
        }
        #endregion
    }
}
