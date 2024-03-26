namespace Aldebaran.Application.Services.Models.Reports
{
    public class InProcessInventoryReport
    {
        public short LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; }
        public int InProcessAmount { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Amount { get; set; }
    }
}
