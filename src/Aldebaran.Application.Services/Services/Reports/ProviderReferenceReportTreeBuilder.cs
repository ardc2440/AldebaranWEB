using Aldebaran.Application.Services.Models.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Arma el árbol Proveedor -> Línea -> Artículo -> Referencias -> Bodegas a partir de las filas planas del SP
    /// (una fila por proveedor, referencia y bodega; sin bodega cuando no se incluye el inventario).
    /// Agrupación con lookups: una sola pasada por nivel (O(n)).
    /// </summary>
    internal static class ProviderReferenceReportTreeBuilder
    {
        public static ProviderReferenceReportResult Build(IEnumerable<Entities.ProviderReferenceReport>? rows, bool includeInventory)
        {
            var data = rows?.ToList() ?? new List<Entities.ProviderReferenceReport>();
            return data.Count == 0
                ? ProviderReferenceReportResult.Empty(includeInventory)
                : new ProviderReferenceReportResult(includeInventory, BuildProviders(data, includeInventory));
        }

        private static IReadOnlyList<ProviderReferenceReportProvider> BuildProviders(IReadOnlyCollection<Entities.ProviderReferenceReport> rows, bool includeInventory)
        {
            var rowsByReference = rows.ToLookup(r => (r.ProviderId, r.ReferenceId));

            return rows.GroupBy(r => r.ProviderId)
                       .Select(g => (First: g.First(), Rows: g))
                       .OrderBy(p => p.First.ProviderName)
                       .Select(p => ToProvider(p.First, BuildLines(p.Rows, rowsByReference, includeInventory)))
                       .ToList();
        }

        private static IReadOnlyList<ProviderReferenceReportLine> BuildLines(IEnumerable<Entities.ProviderReferenceReport> providerRows,
            ILookup<(int, int), Entities.ProviderReferenceReport> rowsByReference, bool includeInventory)
        {
            return providerRows.GroupBy(r => r.LineId)
                               .Select(g => (First: g.First(), Rows: g))
                               .OrderBy(l => l.First.LineName)
                               .Select(l => new ProviderReferenceReportLine(l.First.LineId, l.First.LineCode, l.First.LineName,
                                                                            BuildItems(l.Rows, rowsByReference, includeInventory)))
                               .ToList();
        }

        private static IReadOnlyList<ProviderReferenceReportItem> BuildItems(IEnumerable<Entities.ProviderReferenceReport> lineRows,
            ILookup<(int, int), Entities.ProviderReferenceReport> rowsByReference, bool includeInventory)
        {
            return lineRows.GroupBy(r => r.ItemId)
                           .Select(g => (First: g.First(), Rows: g))
                           .OrderBy(i => i.First.ItemName)
                           .Select(i => new ProviderReferenceReportItem(i.First.ItemId, i.First.InternalReference, i.First.ItemName,
                                                                        BuildReferences(i.Rows, rowsByReference, includeInventory)))
                           .ToList();
        }

        private static IReadOnlyList<ProviderReferenceReportReference> BuildReferences(IEnumerable<Entities.ProviderReferenceReport> itemRows,
            ILookup<(int, int), Entities.ProviderReferenceReport> rowsByReference, bool includeInventory)
        {
            return itemRows.DistinctBy(r => r.ReferenceId)
                           .OrderBy(r => r.ReferenceName)
                           .Select(r => ToReference(r, includeInventory
                               ? BuildWarehouses(rowsByReference[(r.ProviderId, r.ReferenceId)])
                               : Array.Empty<ProviderReferenceReportWarehouse>()))
                           .ToList();
        }

        internal static IReadOnlyList<ProviderReferenceReportWarehouse> BuildWarehouses(IEnumerable<Entities.ProviderReferenceReport> referenceRows)
        {
            return referenceRows.Where(r => r.WarehouseId.HasValue)
                                .DistinctBy(r => r.WarehouseId)
                                .OrderBy(r => r.WarehouseName)
                                .Select(r => new ProviderReferenceReportWarehouse(r.WarehouseId!.Value, r.WarehouseName ?? string.Empty, r.Amount ?? 0))
                                .ToList();
        }

        private static ProviderReferenceReportProvider ToProvider(Entities.ProviderReferenceReport row, IReadOnlyList<ProviderReferenceReportLine> lines) =>
            new(row.ProviderId, row.ProviderCode, row.ProviderName, row.ProviderAddress, row.Phone, row.Fax, row.Email, row.ContactPerson, lines);

        private static ProviderReferenceReportReference ToReference(Entities.ProviderReferenceReport row, IReadOnlyList<ProviderReferenceReportWarehouse> warehouses) =>
            new(row.ReferenceId, row.ReferenceCode, row.ReferenceName, row.ProviderReferenceName,
                row.ConfirmedAmount, row.ReservedAmount, row.AvailableAmount, warehouses);
    }
}
