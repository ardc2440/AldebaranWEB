namespace Aldebaran.DataAccess.Entities
{
    public class ShipmentForwarderAgentMethod
    {
        public short ShipmentForwarderAgentMethodId { get; set; }
        public short ShipmentMethodId { get; set; }
        public int ForwarderAgentId { get; set; }
        // Reverse navigation
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ForwarderAgent ForwarderAgent { get; set; }
        public ShipmentMethod ShipmentMethod { get; set; }
        public ShipmentForwarderAgentMethod()
        {
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
