namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderDetail
    {
        public int PurchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ReferenceId { get; set; }
        public short WarehouseId { get; set; }
        public int? ReceivedQuantity { get; set; }
        public int RequestedQuantity { get; set; }
        public ItemReference ItemReference { get; set; } = null!;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public PurchaseOrderDetail()
        {
            ReceivedQuantity = 0;
            RequestedQuantity = 0;
        }
    }
}
