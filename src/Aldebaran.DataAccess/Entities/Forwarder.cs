using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class Forwarder : ITrackeable
    {
        public int ForwarderId { get; set; }
        public string ForwarderName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string ForwarderAddress { get; set; }
        public string Mail1 { get; set; }
        public string Mail2 { get; set; }
        public int CityId { get; set; }
        // Reverse navigation
        public ICollection<ForwarderAgent> ForwarderAgents { get; set; }
        public City City { get; set; }
        public Forwarder()
        {
            ForwarderAgents = new List<ForwarderAgent>();
        }
    }
}
