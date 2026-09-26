using System.ComponentModel;

namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>
    /// Caso de uso "Exportar reporte de Artículos y Referencias a Excel": una fila por referencia,
    /// textos legibles para el usuario (Activo/Inactivo, Sí/No). El orden de las propiedades es el orden de las columnas.
    /// </summary>
    public sealed class ItemReferencesReportExportRow
    {
        [DisplayName("Línea")]
        public string LineName { get; init; } = string.Empty;
        [DisplayName("Nombre artículo")]
        public string ItemName { get; init; } = string.Empty;
        [DisplayName("Referencia interna")]
        public string InternalReference { get; init; } = string.Empty;
        [DisplayName("Nombre para el proveedor")]
        public string ProviderItemName { get; init; } = string.Empty;
        [DisplayName("Referencia Artículo para proveedor")]
        public string ProviderReference { get; init; } = string.Empty;
        [DisplayName("Estado artículo")]
        public string ItemStatus { get; init; } = string.Empty;
        [DisplayName("Visible en página")]
        public string IsCatalogVisible { get; init; } = string.Empty;
        [DisplayName("Producto nacional")]
        public string IsDomesticProduct { get; init; } = string.Empty;
        [DisplayName("Importación especial")]
        public string IsSpecialImport { get; init; } = string.Empty;
        [DisplayName("Promoción")]
        public string IsSaleOff { get; init; } = string.Empty;
        [DisplayName("Nombre referencia")]
        public string ReferenceName { get; init; } = string.Empty;
        [DisplayName("Código interno")]
        public string ReferenceCode { get; init; } = string.Empty;
        [DisplayName("Nombre Referencia para proveedor")]
        public string ProviderReferenceName { get; init; } = string.Empty;
        [DisplayName("Código Referencia para proveedor")]
        public string ProviderReferenceCode { get; init; } = string.Empty;
        [DisplayName("Estado referencia")]
        public string ReferenceStatus { get; init; } = string.Empty;
        [DisplayName("Referencia agotada")]
        public string IsSoldOut { get; init; } = string.Empty;
        [DisplayName("Cantidad mínima general")]
        public int AlarmMinimumQuantity { get; init; }
        [DisplayName("Cantidad mínima en Bodega local")]
        public int MinimumLocalWarehouseQuantity { get; init; }
        [DisplayName("Variación en orden de compra")]
        public int PurchaseOrderVariation { get; init; }
    }
}
