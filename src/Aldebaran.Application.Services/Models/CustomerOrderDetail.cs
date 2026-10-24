namespace Aldebaran.Application.Services.Models
{
    public class CustomerOrderDetail
    {
        public int CustomerOrderDetailId { get; set; }
        public int CustomerOrderId { get; set; }
        public int ReferenceId { get; set; }
        public int RequestedQuantity { get; set; }
        public int ProcessedQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public string Brand { get; set; } = null!;
        // Reverse navigation
        public ICollection<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        public ICollection<CustomerOrderShipmentDetail> CustomerOrderShipmentDetails { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public ItemReference ItemReference { get; set; } = null!;
        public CustomerOrderDetail()
        {
            ProcessedQuantity = 0;
            DeliveredQuantity = 0;
            CustomerOrderInProcessDetails = new List<CustomerOrderInProcessDetail>();
            CustomerOrderShipmentDetails = new List<CustomerOrderShipmentDetail>();
        }
    }
}
