using Aldebaran.Application.Services.Models.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Arma el árbol Línea -> Artículo -> Referencias a partir de las filas planas del SP.
    /// Agrupación con ToLookup: una sola pasada por nivel (O(n)).
    /// </summary>
    internal static class ItemReferencesReportTreeBuilder
    {
        public static ItemReferencesReportResult Build(IEnumerable<Entities.ItemReferencesReport> rows)
        {
            var data = rows?.ToList() ?? new List<Entities.ItemReferencesReport>();
            return data.Count == 0 ? ItemReferencesReportResult.Empty : new ItemReferencesReportResult(BuildLines(data));
        }

        private static IReadOnlyList<ItemReferencesReportLine> BuildLines(IReadOnlyCollection<Entities.ItemReferencesReport> rows)
        {
            var rowsByItem = rows.ToLookup(r => r.ItemId);

            return rows.GroupBy(r => new { r.LineId, r.LineName })
                       .OrderBy(g => g.Key.LineName)
                       .Select(g => new ItemReferencesReportLine(g.Key.LineId, g.Key.LineName, BuildItems(g, rowsByItem)))
                       .ToList();
        }

        private static IReadOnlyList<ItemReferencesReportItem> BuildItems(IEnumerable<Entities.ItemReferencesReport> lineRows, ILookup<int, Entities.ItemReferencesReport> rowsByItem)
        {
            return lineRows.DistinctBy(r => r.ItemId)
                           .OrderBy(r => r.ItemName)
                           .Select(r => ToItem(r, BuildReferences(rowsByItem[r.ItemId])))
                           .ToList();
        }

        private static IReadOnlyList<ItemReferencesReportReference> BuildReferences(IEnumerable<Entities.ItemReferencesReport> itemRows)
        {
            return itemRows.OrderBy(r => r.ReferenceName)
                           .Select(ToReference)
                           .ToList();
        }

        private static ItemReferencesReportItem ToItem(Entities.ItemReferencesReport row, IReadOnlyList<ItemReferencesReportReference> references) =>
            new(row.ItemId, row.ItemName, row.InternalReference, row.ProviderItemName, row.ProviderReference,
                row.IsItemActive, row.IsCatalogVisible, row.IsDomesticProduct, row.IsSpecialImport, row.IsSaleOff,
                references);

        private static ItemReferencesReportReference ToReference(Entities.ItemReferencesReport row) =>
            new(row.ReferenceId, row.ReferenceName, row.ReferenceCode, row.ProviderReferenceName, row.ProviderReferenceCode,
                row.IsReferenceActive, row.IsSoldOut, row.AlarmMinimumQuantity, row.MinimumLocalWarehouseQuantity, row.PurchaseOrderVariation);
    }
}
