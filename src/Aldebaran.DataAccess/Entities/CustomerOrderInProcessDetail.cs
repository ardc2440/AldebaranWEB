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
        public string Brand { get; set; } = null!;
        public CustomerOrderDetail CustomerOrderDetail { get; set; } = null!;
        public CustomerOrdersInProcess CustomerOrdersInProcess { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
    }
}
