using Aldebaran.Application.Services.Models.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// Convierte las filas del SP en filas de exportación (Excel plano) con textos legibles.
    /// </summary>
    internal static class ItemReferencesReportExportRowBuilder
    {
        private const string Yes = "Sí";
        private const string No = "No";
        private const string Active = "Activo";
        private const string Inactive = "Inactivo";

        public static IReadOnlyList<ItemReferencesReportExportRow> Build(IEnumerable<Entities.ItemReferencesReport> rows)
        {
            return (rows ?? Enumerable.Empty<Entities.ItemReferencesReport>())
                   .OrderBy(r => r.LineName).ThenBy(r => r.ItemName).ThenBy(r => r.ReferenceName)
                   .Select(ToExportRow)
                   .ToList();
        }

        private static ItemReferencesReportExportRow ToExportRow(Entities.ItemReferencesReport row) => new()
        {
            LineName = row.LineName,
            ItemName = row.ItemName,
            InternalReference = row.InternalReference,
            ProviderItemName = row.ProviderItemName,
            ProviderReference = row.ProviderReference,
            ItemStatus = ToStatus(row.IsItemActive),
            IsCatalogVisible = ToYesNo(row.IsCatalogVisible),
            IsDomesticProduct = ToYesNo(row.IsDomesticProduct),
            IsSpecialImport = ToYesNo(row.IsSpecialImport),
            IsSaleOff = ToYesNo(row.IsSaleOff),
            ReferenceName = row.ReferenceName,
            ReferenceCode = row.ReferenceCode,
            ProviderReferenceName = row.ProviderReferenceName ?? string.Empty,
            ProviderReferenceCode = row.ProviderReferenceCode ?? string.Empty,
            ReferenceStatus = ToStatus(row.IsReferenceActive),
            IsSoldOut = ToYesNo(row.IsSoldOut),
            AlarmMinimumQuantity = row.AlarmMinimumQuantity,
            MinimumLocalWarehouseQuantity = row.MinimumLocalWarehouseQuantity,
            PurchaseOrderVariation = row.PurchaseOrderVariation
        };

        private static string ToYesNo(bool value) => value ? Yes : No;

        private static string ToStatus(bool isActive) => isActive ? Active : Inactive;
    }
}
