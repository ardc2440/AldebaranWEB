namespace Aldebaran.Application.Services.Models
{
    public class AdjustmentDetail
    {
        public int AdjustmentDetailId { get; set; }
        public int AdjustmentId { get; set; }
        public int ReferenceId { get; set; }
        public short WarehouseId { get; set; }
        public int Quantity { get; set; }
        public required Adjustment Adjustment { get; set; }
        public required ItemReference ItemReference { get; set; }
        public required Warehouse Warehouse { get; set; }

    }
}
