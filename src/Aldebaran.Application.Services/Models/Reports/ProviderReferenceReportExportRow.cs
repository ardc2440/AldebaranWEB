namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>
    /// Caso de uso "Exportar reporte de Referencias del Proveedor": una fila por proveedor-referencia.
    /// Cantidades y bodegas solo cuando se incluye el inventario (las columnas de bodega se arman
    /// a partir de <see cref="Warehouses"/>, sin depender de Ids fijos).
    /// </summary>
    public sealed class ProviderReferenceReportExportRow
    {
        public string ProviderCode { get; init; } = string.Empty;
        public string ProviderName { get; init; } = string.Empty;
        public string LineName { get; init; } = string.Empty;
        public string InternalReference { get; init; } = string.Empty;
        public string ItemName { get; init; } = string.Empty;
        public string ReferenceCode { get; init; } = string.Empty;
        public string ReferenceName { get; init; } = string.Empty;
        public string ProviderReferenceName { get; init; } = string.Empty;
        public int? ConfirmedAmount { get; init; }
        public int? ReservedAmount { get; init; }
        public int? AvailableAmount { get; init; }
        public IReadOnlyList<ProviderReferenceReportWarehouse> Warehouses { get; init; } = Array.Empty<ProviderReferenceReportWarehouse>();
    }
}
