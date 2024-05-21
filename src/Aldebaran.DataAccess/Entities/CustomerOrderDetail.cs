using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderDetail : ITrackeable
    {
        public int CustomerOrderDetailId { get; set; }
        public int CustomerOrderId { get; set; }
        public int ReferenceId { get; set; }
        public int RequestedQuantity { get; set; }
        public int ProcessedQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public string Brand { get; set; }
        // Reverse navigation
        public ICollection<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        public ICollection<CustomerOrderShipmentDetail> CustomerOrderShipmentDetails { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public ItemReference ItemReference { get; set; }
        public CustomerOrderDetail()
        {
            ProcessedQuantity = 0;
            DeliveredQuantity = 0;
            CustomerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>();
            CustomerOrderShipmentDetails = new List<CustomerOrderShipmentDetail>();
        }
    }
}
