using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Reglas de los switches del reporte de Artículos y Referencias aplicadas en memoria
    /// (ej. para acotar el catálogo del selector). Deben coincidir con las de SP_GET_ITEM_REFERENCES_REPORT:
    /// switch encendido = filtra; apagado = no restringe.
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
            && (!filter.OnlyActiveItems || item.IsActive)
            && (!filter.OnlyCatalogVisible || item.IsCatalogVisible)
            && (!filter.OnlyDomesticProduct || item.IsDomesticProduct)
            && (!filter.OnlySpecialImport || item.IsSpecialImport)
            && (!filter.OnlySaleOff || item.IsSaleOff);

        public static bool MatchesReference(ItemReference reference, ItemReferencesReportFilter filter) =>
            (!filter.OnlyActiveReferences || reference.IsActive)
            && (!filter.OnlySoldOut || reference.IsSoldOut)
            && (!filter.OnlyWithAlarmMinimumQuantity || reference.AlarmMinimumQuantity > 0)
            && (!filter.OnlyWithMinimumLocalWarehouseQuantity || reference.MinimumLocalWarehouseQuantity > 0);
    }
}
