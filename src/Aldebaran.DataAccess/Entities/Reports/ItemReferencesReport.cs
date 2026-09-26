namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Fila de SP_GET_ITEM_REFERENCES_REPORT: una por referencia, con datos de su artículo y línea.</summary>
    public class ItemReferencesReport
    {
        public short LineId { get; set; }
        public string LineName { get; set; } = string.Empty;

        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string InternalReference { get; set; } = string.Empty;
        public string ProviderItemName { get; set; } = string.Empty;
        public string ProviderReference { get; set; } = string.Empty;
        public bool IsItemActive { get; set; }
        public bool IsCatalogVisible { get; set; }
        public bool IsDomesticProduct { get; set; }
        public bool IsSpecialImport { get; set; }
        public bool IsSaleOff { get; set; }

        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; } = string.Empty;
        public string ReferenceCode { get; set; } = string.Empty;
        public string? ProviderReferenceName { get; set; }
        public string? ProviderReferenceCode { get; set; }
        public bool IsReferenceActive { get; set; }
        public bool IsSoldOut { get; set; }
        public int AlarmMinimumQuantity { get; set; }
        public int MinimumLocalWarehouseQuantity { get; set; }
        public int PurchaseOrderVariation { get; set; }
    }
}
