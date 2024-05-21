using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderShipmentDetail : ITrackeable
    {
        public int CustomerOrderShipmentDetailId { get; set; }
        public int CustomerOrderShipmentId { get; set; }
        public int CustomerOrderDetailId { get; set; }
        public int DeliveredQuantity { get; set; }
        public CustomerOrderDetail CustomerOrderDetail { get; set; }
        public CustomerOrderShipment CustomerOrderShipment { get; set; }
    }
}
