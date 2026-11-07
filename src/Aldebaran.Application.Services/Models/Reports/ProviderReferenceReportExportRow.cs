using System.ComponentModel;

namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>
    /// Caso de uso "Exportar reporte de Referencias del Proveedor a Excel".
    /// Trae solo una de las dos listas, según la opción "Mostrar inventario y bodegas" del filtro.
    /// </summary>
    public sealed record ProviderReferenceReportExport(
        bool IncludeInventory,
        IReadOnlyList<ProviderReferenceReportExportRow> References,
        IReadOnlyList<ProviderReferenceReportInventoryExportRow> InventoryReferences)
    {
        public int Count => IncludeInventory ? InventoryReferences.Count : References.Count;
    }

    /// <summary>Fila de Excel sin inventario: una por proveedor-referencia. El orden de las propiedades es el orden de las columnas.</summary>
    public sealed class ProviderReferenceReportExportRow
    {
        [DisplayName("Código proveedor")]
        public string ProviderCode { get; init; } = string.Empty;
        [DisplayName("Proveedor")]
        public string ProviderName { get; init; } = string.Empty;
        [DisplayName("Línea")]
        public string LineName { get; init; } = string.Empty;
        [DisplayName("Referencia interna")]
        public string InternalReference { get; init; } = string.Empty;
        [DisplayName("Artículo")]
        public string ItemName { get; init; } = string.Empty;
        [DisplayName("Código")]
        public string ReferenceCode { get; init; } = string.Empty;
        [DisplayName("Referencia")]
        public string ReferenceName { get; init; } = string.Empty;
        [DisplayName("Referencia proveedor")]
        public string ProviderReferenceName { get; init; } = string.Empty;
    }

    /// <summary>
    /// Fila de Excel con inventario: una por proveedor-referencia, con saldos y cantidad por bodega
    /// (Bodega Local y Zona Franca, como el reporte de Inventario). El orden de las propiedades es el orden de las columnas.
    /// </summary>
    public sealed class ProviderReferenceReportInventoryExportRow
    {
        [DisplayName("Código proveedor")]
        public string ProviderCode { get; init; } = string.Empty;
        [DisplayName("Proveedor")]
        public string ProviderName { get; init; } = string.Empty;
        [DisplayName("Línea")]
        public string LineName { get; init; } = string.Empty;
        [DisplayName("Referencia interna")]
        public string InternalReference { get; init; } = string.Empty;
        [DisplayName("Artículo")]
        public string ItemName { get; init; } = string.Empty;
        [DisplayName("Código")]
        public string ReferenceCode { get; init; } = string.Empty;
        [DisplayName("Referencia")]
        public string ReferenceName { get; init; } = string.Empty;
        [DisplayName("Referencia proveedor")]
        public string ProviderReferenceName { get; init; } = string.Empty;
        [DisplayName("Comprometido")]
        public int ConfirmedAmount { get; init; }
        [DisplayName("Reservado")]
        public int ReservedAmount { get; init; }
        [DisplayName("Existente")]
        public int AvailableAmount { get; init; }
        [DisplayName("Bodega Local")]
        public int LocalWarehouse { get; init; }
        [DisplayName("Zona Franca")]
        public int FreeZone { get; init; }
    }
}
