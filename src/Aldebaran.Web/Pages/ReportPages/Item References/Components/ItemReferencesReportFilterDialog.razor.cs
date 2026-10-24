using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Item_References.Components
{
    /// <summary>
    /// Diálogo de filtros del reporte de Artículos y Referencias.
    /// Condiciones de Artículo y Referencia Sí/No/Todos (casilla de tres estados) y jerarquía
    /// Línea -> Artículo -> Referencia sin nivel obligatorio. Las condiciones también acotan el catálogo
    /// que ofrece el selector, conservando la selección que siga siendo válida.
    /// Devuelve un ItemReferencesReportFilterResult (filtro + resumen legible) o null si se cancela.
    /// </summary>
    public partial class ItemReferencesReportFilterDialog
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        /// <summary>Filtro vigente del reporte, para restaurar switches y selección al reabrir el diálogo.</summary>
        [Parameter]
        public ItemReferencesReportFilter Filter { get; set; }
        #endregion

        #region Variables
        protected HierarchicalReferencePicker referencePicker;
        protected bool IsLoadingReferences = true;

        /// <summary>Copia de trabajo de los switches; el filtro vigente no se altera si se cancela.</summary>
        protected ItemReferencesReportFilter Criteria = new();
        protected IReadOnlyList<ConditionOption> ItemSwitches = Array.Empty<ConditionOption>();
        protected IReadOnlyList<ConditionOption> ReferenceSwitches = Array.Empty<ConditionOption>();

        private List<ItemReference> _catalog = new();
        #endregion

        #region Overrides
        protected override void OnParametersSet()
        {
            Filter ??= new ItemReferencesReportFilter();
            Criteria = CopySwitches(Filter, new ItemReferencesReportFilter());
            BuildSwitchOptions();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;
            await LoadPickerAsync();
        }
        #endregion

        #region Condiciones
        private void BuildSwitchOptions()
        {
            ItemSwitches = new List<ConditionOption>
            {
                new("Activo", () => Criteria.IsItemActive, v => Criteria.IsItemActive = v),
                new("Visible en página", () => Criteria.IsCatalogVisible, v => Criteria.IsCatalogVisible = v),
                new("Producto nacional", () => Criteria.IsDomesticProduct, v => Criteria.IsDomesticProduct = v),
                new("Importación especial", () => Criteria.IsSpecialImport, v => Criteria.IsSpecialImport = v),
                new("En promoción", () => Criteria.IsSaleOff, v => Criteria.IsSaleOff = v),
                new("Precio bomba", () => Criteria.BombPrice, v => Criteria.BombPrice = v)
            };
            ReferenceSwitches = new List<ConditionOption>
            {
                new("Activa", () => Criteria.IsReferenceActive, v => Criteria.IsReferenceActive = v),
                new("Agotada", () => Criteria.IsSoldOut, v => Criteria.IsSoldOut = v),
                new("Con cantidad mínima general", () => Criteria.HasAlarmMinimumQuantity, v => Criteria.HasAlarmMinimumQuantity = v),
                new("Con cantidad mínima en Bodega Local", () => Criteria.HasMinimumLocalWarehouseQuantity, v => Criteria.HasMinimumLocalWarehouseQuantity = v)
            };
        }

        /// <summary>Aplica la condición (Sí/No/Todos) y acota el catálogo del selector conservando la selección válida.</summary>
        protected void OnConditionChanged(ConditionOption option, bool? value)
        {
            option.Set(value);
            ApplyCatalog(referencePicker.GetSelectedLineIds(), referencePicker.GetSelectedItemIds(), referencePicker.GetSelectedReferenceIds());
        }

        private static ItemReferencesReportFilter CopySwitches(ItemReferencesReportFilter source, ItemReferencesReportFilter target)
        {
            target.IsItemActive = source.IsItemActive;
            target.IsCatalogVisible = source.IsCatalogVisible;
            target.IsDomesticProduct = source.IsDomesticProduct;
            target.IsSpecialImport = source.IsSpecialImport;
            target.IsSaleOff = source.IsSaleOff;
            target.BombPrice = source.BombPrice;
            target.IsReferenceActive = source.IsReferenceActive;
            target.IsSoldOut = source.IsSoldOut;
            target.HasAlarmMinimumQuantity = source.HasAlarmMinimumQuantity;
            target.HasMinimumLocalWarehouseQuantity = source.HasMinimumLocalWarehouseQuantity;
            return target;
        }

        /// <summary>Condición de tres estados: null = Todos, true = Sí, false = No.</summary>
        protected sealed record ConditionOption(string Label, Func<bool?> Get, Action<bool?> Set);
        #endregion

        #region Picker
        /// <summary>Carga el catálogo una sola vez y lo entrega al selector con la selección vigente.</summary>
        private async Task LoadPickerAsync()
        {
            _catalog = (await ItemReferenceService.GetReportsReferencesAsync()).ToList();
            ApplyCatalog(Filter.LineIds, Filter.ItemIds, Filter.ReferenceIds);
            IsLoadingReferences = false;
            StateHasChanged();
        }

        /// <summary>Entrega al selector el catálogo que cumple los switches y restaura la selección que siga siendo válida.</summary>
        private void ApplyCatalog(IEnumerable<short> lineIds, IEnumerable<int> itemIds, IEnumerable<int> referenceIds)
        {
            referencePicker.SetAvailableItemReferencesForSelection(ItemReferencesReportSwitchRules.Apply(_catalog, Criteria).ToList());
            referencePicker.RestoreSelection(lineIds, itemIds, referenceIds);
        }
        #endregion

        #region Events
        protected void FormSubmit()
        {
            var filter = BuildFilterFromSelection();
            DialogService.Close(new ItemReferencesReportFilterResult(filter, BuildSummary(filter)));
        }

        protected void CancelButtonClick() => DialogService.Close(null);

        private ItemReferencesReportFilterSummary BuildSummary(ItemReferencesReportFilter filter) =>
            ItemReferencesReportFilterSummaryBuilder.Build(filter, _catalog, ActiveSwitchLabels());

        /// <summary>Condiciones aplicadas (Sí/No) para el resumen; Todos no se muestra.</summary>
        private IEnumerable<string> ActiveSwitchLabels() =>
            ItemSwitches.Concat(ReferenceSwitches)
                        .Where(o => o.Get().HasValue)
                        .Select(o => $"{o.Label}: {(o.Get()!.Value ? "Sí" : "No")}")
                        .ToList();

        private ItemReferencesReportFilter BuildFilterFromSelection()
        {
            var filter = CopySwitches(Criteria, new ItemReferencesReportFilter());
            filter.LineIds = referencePicker.GetSelectedLineIds();
            filter.ItemIds = referencePicker.GetSelectedItemIds();
            filter.ReferenceIds = referencePicker.GetSelectedReferenceIds();
            return filter;
        }
        #endregion
    }
}
