using Aldebaran.Application.Services.Models.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Convierte las filas del SP (una por proveedor, referencia y bodega) en filas de Excel:
    /// una por proveedor-referencia, con o sin inventario según la opción del filtro.
    /// </summary>
    internal static class ProviderReferenceReportExportRowBuilder
    {
        /// <summary>Códigos de bodega (warehouses.WAREHOUSE_ID = WAREHOUSE_CODE): 1 Bodega Local, 2 Zona Franca.</summary>
        internal const short LocalWarehouseId = 1;
        internal const short FreeZoneWarehouseId = 2;

        public static ProviderReferenceReportExport Build(IEnumerable<Entities.ProviderReferenceReport>? rows, bool includeInventory)
        {
            var groups = (rows ?? Enumerable.Empty<Entities.ProviderReferenceReport>())
                         .GroupBy(r => (r.ProviderId, r.ReferenceId))
                         .Select(g => (Row: g.First(), Rows: g))
                         .OrderBy(g => g.Row.ProviderName).ThenBy(g => g.Row.LineName).ThenBy(g => g.Row.ItemName).ThenBy(g => g.Row.ReferenceName)
                         .ToList();

            return includeInventory
                ? new ProviderReferenceReportExport(true, Array.Empty<ProviderReferenceReportExportRow>(), groups.Select(g => ToInventoryRow(g.Row, g.Rows)).ToList())
                : new ProviderReferenceReportExport(false, groups.Select(g => ToRow(g.Row)).ToList(), Array.Empty<ProviderReferenceReportInventoryExportRow>());
        }

        private static ProviderReferenceReportExportRow ToRow(Entities.ProviderReferenceReport row) => new()
        {
            ProviderCode = row.ProviderCode,
            ProviderName = row.ProviderName,
            LineName = row.LineName,
            InternalReference = row.InternalReference,
            ItemName = row.ItemName,
            ReferenceCode = row.ReferenceCode,
            ReferenceName = row.ReferenceName,
            ProviderReferenceName = row.ProviderReferenceName ?? string.Empty
        };

        private static ProviderReferenceReportInventoryExportRow ToInventoryRow(Entities.ProviderReferenceReport row, IEnumerable<Entities.ProviderReferenceReport> referenceRows) => new()
        {
            ProviderCode = row.ProviderCode,
            ProviderName = row.ProviderName,
            LineName = row.LineName,
            InternalReference = row.InternalReference,
            ItemName = row.ItemName,
            ReferenceCode = row.ReferenceCode,
            ReferenceName = row.ReferenceName,
            ProviderReferenceName = row.ProviderReferenceName ?? string.Empty,
            ConfirmedAmount = row.ConfirmedAmount,
            ReservedAmount = row.ReservedAmount,
            AvailableAmount = row.AvailableAmount,
            LocalWarehouse = Quantity(referenceRows, LocalWarehouseId),
            FreeZone = Quantity(referenceRows, FreeZoneWarehouseId)
        };

        private static int Quantity(IEnumerable<Entities.ProviderReferenceReport> referenceRows, short warehouseId) =>
            referenceRows.FirstOrDefault(r => r.WarehouseId == warehouseId)?.Amount ?? 0;
    }
}
