using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderInProcessDetail : ITrackeable
    {
        public int CustomerOrderInProcessDetailId { get; set; }
        public int CustomerOrderInProcessId { get; set; }
        public int CustomerOrderDetailId { get; set; }
        public short WarehouseId { get; set; }
        public int ProcessedQuantity { get; set; }
        public string Brand { get; set; }
        public CustomerOrderDetail CustomerOrderDetail { get; set; }
        public CustomerOrdersInProcess CustomerOrdersInProcess { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
