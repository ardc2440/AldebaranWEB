using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class ForwarderAgent : ITrackeable
    {
        public int ForwarderAgentId { get; set; }
        public int ForwarderId { get; set; }
        public string ForwarderAgentName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string ForwarderAgentAddress { get; set; }
        public int CityId { get; set; }
        public string Contact { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        // Reverse navigation
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }
        public City City { get; set; }
        public Forwarder Forwarder { get; set; }
        public ForwarderAgent()
        {
            PurchaseOrders = new List<PurchaseOrder>();
            ShipmentForwarderAgentMethods = new List<ShipmentForwarderAgentMethod>();
        }
    }
}
