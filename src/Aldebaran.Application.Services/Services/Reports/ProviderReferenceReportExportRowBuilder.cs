using Aldebaran.Application.Services.Models.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Convierte las filas del SP (una por proveedor, referencia y bodega) en filas de exportación:
    /// una por proveedor-referencia, con sus bodegas cuando se incluye el inventario.
    /// </summary>
    internal static class ProviderReferenceReportExportRowBuilder
    {
        public static IReadOnlyList<ProviderReferenceReportExportRow> Build(IEnumerable<Entities.ProviderReferenceReport>? rows, bool includeInventory)
        {
            return (rows ?? Enumerable.Empty<Entities.ProviderReferenceReport>())
                   .GroupBy(r => (r.ProviderId, r.ReferenceId))
                   .Select(g => ToExportRow(g.First(), g, includeInventory))
                   .OrderBy(r => r.ProviderName).ThenBy(r => r.LineName).ThenBy(r => r.ItemName).ThenBy(r => r.ReferenceName)
                   .ToList();
        }

        private static ProviderReferenceReportExportRow ToExportRow(Entities.ProviderReferenceReport row,
            IEnumerable<Entities.ProviderReferenceReport> referenceRows, bool includeInventory) => new()
        {
            ProviderCode = row.ProviderCode,
            ProviderName = row.ProviderName,
            LineName = row.LineName,
            InternalReference = row.InternalReference,
            ItemName = row.ItemName,
            ReferenceCode = row.ReferenceCode,
            ReferenceName = row.ReferenceName,
            ProviderReferenceName = row.ProviderReferenceName ?? string.Empty,
            ConfirmedAmount = includeInventory ? row.ConfirmedAmount : null,
            ReservedAmount = includeInventory ? row.ReservedAmount : null,
            AvailableAmount = includeInventory ? row.AvailableAmount : null,
            Warehouses = includeInventory
                ? ProviderReferenceReportTreeBuilder.BuildWarehouses(referenceRows)
                : Array.Empty<ProviderReferenceReportWarehouse>()
        };
    }
}
