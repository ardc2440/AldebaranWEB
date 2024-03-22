namespace Aldebaran.DataAccess.Entities
{
    public class InProcessInventoryReport
    {
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public string ReferenceName { get; set; }
        public int InProcessAmount { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Amount { get; set; }
    }
}
