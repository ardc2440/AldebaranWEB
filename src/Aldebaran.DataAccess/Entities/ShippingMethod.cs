namespace Aldebaran.DataAccess.Entities
{
    public class ShippingMethod
    {
        public short ShippingMethodId { get; set; }
        public string ShippingMethodName { get; set; }
        public string ShippingMethodNotes { get; set; }
        // Reverse navigation
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public ShippingMethod()
        {
            CustomerOrderShipments = new List<CustomerOrderShipment>();
        }
    }
}
