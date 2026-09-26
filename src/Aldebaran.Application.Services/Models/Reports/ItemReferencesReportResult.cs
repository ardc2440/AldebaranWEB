namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>
    /// Caso de uso "Consultar reporte de Artículos y Referencias": resultado ya organizado
    /// en árbol Línea -> Artículo -> Referencias, listo para pantalla, impresión y PDF.
    /// </summary>
    public sealed record ItemReferencesReportResult(IReadOnlyList<ItemReferencesReportLine> Lines)
    {
        public static ItemReferencesReportResult Empty { get; } = new(Array.Empty<ItemReferencesReportLine>());
        public bool HasData => Lines.Count > 0;
    }

    public sealed record ItemReferencesReportLine(
        short LineId,
        string LineName,
        IReadOnlyList<ItemReferencesReportItem> Items);

    public sealed record ItemReferencesReportItem(
        int ItemId,
        string ItemName,
        string InternalReference,
        string ProviderItemName,
        string ProviderReference,
        bool IsActive,
        bool IsCatalogVisible,
        bool IsDomesticProduct,
        bool IsSpecialImport,
        bool IsSaleOff,
        IReadOnlyList<ItemReferencesReportReference> References);

    public sealed record ItemReferencesReportReference(
        int ReferenceId,
        string ReferenceName,
        string ReferenceCode,
        string? ProviderReferenceName,
        string? ProviderReferenceCode,
        bool IsActive,
        bool IsSoldOut,
        int AlarmMinimumQuantity,
        int MinimumLocalWarehouseQuantity,
        int PurchaseOrderVariation);
}
