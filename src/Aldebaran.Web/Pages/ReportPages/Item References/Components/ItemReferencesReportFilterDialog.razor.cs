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
    /// Switches de Artículo y Referencia (encendido = filtra, apagado = trae todo) y jerarquía
    /// Línea -> Artículo -> Referencia sin nivel obligatorio. Los switches también acotan el catálogo
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
        protected IReadOnlyList<SwitchOption> ItemSwitches = Array.Empty<SwitchOption>();
        protected IReadOnlyList<SwitchOption> ReferenceSwitches = Array.Empty<SwitchOption>();

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

        #region Switches
        private void BuildSwitchOptions()
        {
            ItemSwitches = new List<SwitchOption>
            {
                new("Solo activos", () => Criteria.OnlyActiveItems, v => Criteria.OnlyActiveItems = v),
                new("Solo visibles en página", () => Criteria.OnlyCatalogVisible, v => Criteria.OnlyCatalogVisible = v),
                new("Solo producto nacional", () => Criteria.OnlyDomesticProduct, v => Criteria.OnlyDomesticProduct = v),
                new("Solo importación especial", () => Criteria.OnlySpecialImport, v => Criteria.OnlySpecialImport = v),
                new("Solo en promoción", () => Criteria.OnlySaleOff, v => Criteria.OnlySaleOff = v)
            };
            ReferenceSwitches = new List<SwitchOption>
            {
                new("Solo referencias activas", () => Criteria.OnlyActiveReferences, v => Criteria.OnlyActiveReferences = v),
                new("Solo referencias agotadas", () => Criteria.OnlySoldOut, v => Criteria.OnlySoldOut = v),
                new("Solo con cantidad mínima general", () => Criteria.OnlyWithAlarmMinimumQuantity, v => Criteria.OnlyWithAlarmMinimumQuantity = v),
                new("Solo con cantidad mínima en Bodega Local", () => Criteria.OnlyWithMinimumLocalWarehouseQuantity, v => Criteria.OnlyWithMinimumLocalWarehouseQuantity = v)
            };
        }

        /// <summary>Aplica el switch y acota el catálogo del selector conservando la selección válida.</summary>
        protected void OnSwitchChanged(SwitchOption option, bool value)
        {
            option.Set(value);
            ApplyCatalog(referencePicker.GetSelectedLineIds(), referencePicker.GetSelectedItemIds(), referencePicker.GetSelectedReferenceIds());
        }

        private static ItemReferencesReportFilter CopySwitches(ItemReferencesReportFilter source, ItemReferencesReportFilter target)
        {
            target.OnlyActiveItems = source.OnlyActiveItems;
            target.OnlyCatalogVisible = source.OnlyCatalogVisible;
            target.OnlyDomesticProduct = source.OnlyDomesticProduct;
            target.OnlySpecialImport = source.OnlySpecialImport;
            target.OnlySaleOff = source.OnlySaleOff;
            target.OnlyActiveReferences = source.OnlyActiveReferences;
            target.OnlySoldOut = source.OnlySoldOut;
            target.OnlyWithAlarmMinimumQuantity = source.OnlyWithAlarmMinimumQuantity;
            target.OnlyWithMinimumLocalWarehouseQuantity = source.OnlyWithMinimumLocalWarehouseQuantity;
            return target;
        }

        protected sealed record SwitchOption(string Label, Func<bool> Get, Action<bool> Set);
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

        private IEnumerable<string> ActiveSwitchLabels() =>
            ItemSwitches.Concat(ReferenceSwitches).Where(o => o.Get()).Select(o => o.Label);

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
