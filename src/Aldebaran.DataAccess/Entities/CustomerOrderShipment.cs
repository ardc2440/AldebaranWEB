using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderShipment : ITrackeable
    {
        public int CustomerOrderShipmentId { get; set; }
        public int CustomerOrderId { get; set; }
        public short ShippingMethodId { get; set; }
        public string Notes { get; set; }
        public string TrackingNumber { get; set; }
        public string DeliveryNote { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime CreationDate { get; set; }
        public short StatusDocumentTypeId { get; set; }

        // Reverse navigation
        public CanceledOrderShipment CanceledOrderShipment { get; set; }
        public ICollection<CustomerOrderShipmentDetail> CustomerOrderShipmentDetails { get; set; }
        public ICollection<ModifiedOrderShipment> ModifiedOrderShipments { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public CustomerOrderShipment()
        {
            ShippingDate = DateTime.Now;
            CreationDate = DateTime.Now;
            CustomerOrderShipmentDetails = new List<CustomerOrderShipmentDetail>();
            ModifiedOrderShipments = new List<ModifiedOrderShipment>();
        }
    }
}
