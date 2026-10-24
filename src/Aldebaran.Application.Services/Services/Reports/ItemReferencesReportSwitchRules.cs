using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Reglas de los switches del reporte de Artículos y Referencias aplicadas en memoria
    /// (ej. para acotar el catálogo del selector). Deben coincidir con las de SP_GET_ITEM_REFERENCES_REPORT:
    /// condición Sí/No filtra; Todos (null) no restringe.
    /// </summary>
    public static class ItemReferencesReportSwitchRules
    {
        public static IEnumerable<ItemReference> Apply(IEnumerable<ItemReference> references, ItemReferencesReportFilter filter)
        {
            filter ??= new ItemReferencesReportFilter();
            return (references ?? Enumerable.Empty<ItemReference>())
                   .Where(r => MatchesItem(r.Item, filter) && MatchesReference(r, filter));
        }

        public static bool MatchesItem(Item item, ItemReferencesReportFilter filter) =>
            item != null
            && Matches(filter.IsItemActive, item.IsActive)
            && Matches(filter.IsCatalogVisible, item.IsCatalogVisible)
            && Matches(filter.IsDomesticProduct, item.IsDomesticProduct)
            && Matches(filter.IsSpecialImport, item.IsSpecialImport)
            && Matches(filter.IsSaleOff, item.IsSaleOff)
            && Matches(filter.BombPrice, item.IsBombPrice);

        public static bool MatchesReference(ItemReference reference, ItemReferencesReportFilter filter) =>
            Matches(filter.IsReferenceActive, reference.IsActive)
            && Matches(filter.IsSoldOut, reference.IsSoldOut)
            && Matches(filter.HasAlarmMinimumQuantity, reference.AlarmMinimumQuantity > 0)
            && Matches(filter.HasMinimumLocalWarehouseQuantity, reference.MinimumLocalWarehouseQuantity > 0);

        /// <summary>Condición Sí/No/Todos: null no restringe.</summary>
        private static bool Matches(bool? condition, bool value) => condition == null || condition.Value == value;
    }
}
