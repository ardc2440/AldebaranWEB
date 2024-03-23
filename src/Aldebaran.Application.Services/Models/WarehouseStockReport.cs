namespace Aldebaran.Application.Services.Models
{
    public class WarehouseStockReport
    {
        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public short LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public int ReferenceId { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public string ProviderReferenceName { get; set; }
        public int AvailableAmount { get; set; }
    }
}
