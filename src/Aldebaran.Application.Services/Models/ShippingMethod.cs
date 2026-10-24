namespace Aldebaran.Application.Services.Models
{
    public class ShippingMethod
    {
        public short ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; } = null!;
        public string ShippingMethodNotes { get; set; } = null!;
        // Reverse navigation
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public ShippingMethod()
        {
            CustomerOrderShipments = new List<CustomerOrderShipment>();
        }
    }
}
