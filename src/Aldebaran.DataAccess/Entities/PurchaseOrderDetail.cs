using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderDetail : ITrackeable
    {
        public int PurchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ReferenceId { get; set; }
        public short WarehouseId { get; set; }
        public int? ReceivedQuantity { get; set; }
        public int RequestedQuantity { get; set; }
        public ItemReference ItemReference { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public Warehouse Warehouse { get; set; }
        public PurchaseOrderDetail()
        {
            ReceivedQuantity = 0;
            RequestedQuantity = 0;
        }
    }
}
