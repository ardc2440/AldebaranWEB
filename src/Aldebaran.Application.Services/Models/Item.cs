namespace Aldebaran.Application.Services.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public short LineId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }
        public string ProviderReference { get; set; }
        public string ProviderItemName { get; set; }
        public double FobCost { get; set; }
        public short CurrencyId { get; set; }
        public string Notes { get; set; }
        public bool IsExternalInventory { get; set; }
        public double CifCost { get; set; }
        public double Volume { get; set; }
        public double Weight { get; set; }
        public short FobMeasureUnitId { get; set; }
        public short? CifMeasureUnitId { get; set; }
        public bool IsDomesticProduct { get; set; }
        public bool IsActive { get; set; }
        public bool IsCatalogVisible { get; set; }
        public bool IsSpecialImport { get; set; }
        public bool IsSaleOff {  get; set; }

        // Reverse navigation
        public ICollection<ItemReference> ItemReferences { get; set; }
        public ICollection<ItemsArea> ItemsAreas { get; set; }
        public ICollection<Packaging> Packagings { get; set; }
        public Currency Currency { get; set; }
        public Line Line { get; set; }
        public MeasureUnit CifMeasureUnit { get; set; }
        public MeasureUnit FobMeasureUnit { get; set; }
        public Item()
        {
            IsExternalInventory = false;
            IsDomesticProduct = false;
            IsActive = true;
            IsCatalogVisible = false;
            IsSpecialImport = false;
            ItemReferences = new List<ItemReference>();
            ItemsAreas = new List<ItemsArea>();
            Packagings = new List<Packaging>();
        }
    }
}
