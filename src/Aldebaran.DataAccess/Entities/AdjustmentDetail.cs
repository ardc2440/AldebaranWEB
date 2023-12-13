namespace Aldebaran.DataAccess.Entities
{
    public class AdjustmentDetail
    {
        public int AdjustmentDetailId { get; set; }
        public int AdjustmentId { get; set; }
        public int ReferenceId { get; set; }
        public short WarehouseId { get; set; }
        public int Quantity { get; set; }
        public Adjustment Adjustment { get; set; }
        public ItemReference ItemReference { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
