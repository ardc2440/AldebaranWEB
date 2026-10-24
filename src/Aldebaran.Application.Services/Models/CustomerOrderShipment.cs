namespace Aldebaran.Application.Services.Models
{
    public class CustomerOrderShipment
    {
        public int CustomerOrderShipmentId { get; set; }
        public int CustomerOrderId { get; set; }
        public short ShippingMethodId { get; set; }
        public string Notes { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public string DeliveryNote { get; set; } = null!;
        public int EmployeeId { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime CreationDate { get; set; }
        public short StatusDocumentTypeId { get; set; }

        // Reverse navigation
        public CanceledOrderShipment CanceledOrderShipment { get; set; } = null!;
        public ICollection<CustomerOrderShipmentDetail> CustomerOrderShipmentDetails { get; set; }
        public ICollection<ModifiedOrderShipment> ModifiedOrderShipments { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ShippingMethod ShippingMethod { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;

        public CustomerOrderShipment()
        {
            ShippingDate = DateTime.Now;
            CreationDate = DateTime.Now;
            CustomerOrderShipmentDetails = new List<CustomerOrderShipmentDetail>();
            ModifiedOrderShipments = new List<ModifiedOrderShipment>();
        }
    }
}
