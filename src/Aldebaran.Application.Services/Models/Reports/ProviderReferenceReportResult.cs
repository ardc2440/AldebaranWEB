namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>
    /// Caso de uso "Consultar reporte de Referencias del Proveedor": resultado organizado en árbol
    /// Proveedor -> Línea -> Artículo -> Referencias (-> Bodegas cuando se incluye el inventario),
    /// listo para pantalla, impresión y PDF.
    /// </summary>
    public sealed record ProviderReferenceReportResult(bool IncludeInventory, IReadOnlyList<ProviderReferenceReportProvider> Providers)
    {
        public static ProviderReferenceReportResult Empty(bool includeInventory) => new(includeInventory, Array.Empty<ProviderReferenceReportProvider>());
        public bool HasData => Providers.Count > 0;
    }

    public sealed record ProviderReferenceReportProvider(
        int ProviderId,
        string ProviderCode,
        string ProviderName,
        string? ProviderAddress,
        string? Phone,
        string? Fax,
        string? Email,
        string? ContactPerson,
        IReadOnlyList<ProviderReferenceReportLine> Lines);

    public sealed record ProviderReferenceReportLine(
        short LineId,
        string LineCode,
        string LineName,
        IReadOnlyList<ProviderReferenceReportItem> Items);

    public sealed record ProviderReferenceReportItem(
        int ItemId,
        string InternalReference,
        string ItemName,
        IReadOnlyList<ProviderReferenceReportReference> References);

    /// <summary>Referencia del proveedor. Cantidades y bodegas solo cuando el reporte incluye inventario.</summary>
    public sealed record ProviderReferenceReportReference(
        int ReferenceId,
        string ReferenceCode,
        string ReferenceName,
        string? ProviderReferenceName,
        int ConfirmedAmount,
        int ReservedAmount,
        int AvailableAmount,
        IReadOnlyList<ProviderReferenceReportWarehouse> Warehouses);

    public sealed record ProviderReferenceReportWarehouse(short WarehouseId, string WarehouseName, int Amount);
}
