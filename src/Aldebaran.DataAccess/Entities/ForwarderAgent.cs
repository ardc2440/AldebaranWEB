using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class ForwarderAgent : ITrackeable
    {
        public int ForwarderAgentId { get; set; }
        public int ForwarderId { get; set; }
        public string ForwarderAgentName { get; set; } = null!;
        public string Phone1 { get; set; } = null!;
        public string Phone2 { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string ForwarderAgentAddress { get; set; } = null!;
        public int CityId { get; set; }
        public string Contact { get; set; } = null!;
        public string Email1 { get; set; } = null!;
        public string Email2 { get; set; } = null!;
        // Reverse navigation
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }
        public City City { get; set; } = null!;
        public Forwarder Forwarder { get; set; } = null!;
        public ForwarderAgent()
        {
            PurchaseOrders = new List<PurchaseOrder>();
            ShipmentForwarderAgentMethods = new List<ShipmentForwarderAgentMethod>();
        }
    }
}
