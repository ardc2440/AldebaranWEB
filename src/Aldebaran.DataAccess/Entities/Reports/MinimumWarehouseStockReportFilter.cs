namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Filtros del reporte de existencias mínimas en bodega (null = no filtra).</summary>
    public class MinimumWarehouseStockReportFilter
    {
        /// <summary>Umbral máximo de existencias (obligatorio).</summary>
        public int MaximumQuantityThreshold { get; set; }
        public short? WarehouseId { get; set; }
        /// <summary>Ids de artículo separados por coma.</summary>
        public string? ItemIds { get; set; }
        /// <summary>Ids de referencia separados por coma.</summary>
        public string? ReferenceIds { get; set; }
        public string? ProviderItemName { get; set; }
        public string? ProviderReference { get; set; }
        public string? ProviderReferenceName { get; set; }
        public string? ProviderReferenceCode { get; set; }
        public bool? ItemIsActive { get; set; }
        public bool? ItemIsDomesticProduct { get; set; }
        public bool? ItemIsSpecialImport { get; set; }
        public bool? ItemIsSaleOff { get; set; }
        public bool? ItemIsCatalogVisible { get; set; }
        public bool? ItemIsBombPrice { get; set; }
        public bool? ReferenceIsActive { get; set; }
        public bool? ReferenceHasAlarmMinimumQuantity { get; set; }
        public bool? ReferenceIsSoldOut { get; set; }
    }
}
