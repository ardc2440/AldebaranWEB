using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class ShipmentForwarderAgentMethod : ITrackeable
    {
        public short ShipmentForwarderAgentMethodId { get; set; }
        public short ShipmentMethodId { get; set; }
        public int ForwarderAgentId { get; set; }
        // Reverse navigation
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ForwarderAgent ForwarderAgent { get; set; } = null!;
        public ShipmentMethod ShipmentMethod { get; set; } = null!;
        public ShipmentForwarderAgentMethod()
        {
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
