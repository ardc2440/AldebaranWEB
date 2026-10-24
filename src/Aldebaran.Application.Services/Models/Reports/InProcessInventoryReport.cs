namespace Aldebaran.Application.Services.Models.Reports
{
    public class InProcessInventoryReport
    {
        public short LineId { get; set; }
        public string LineCode { get; set; } = null!;
        public string LineName { get; set; } = null!;

        public int ItemId { get; set; }
        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;

        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; } = null!;
        public int InProcessAmount { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int Amount { get; set; }
    }
}
