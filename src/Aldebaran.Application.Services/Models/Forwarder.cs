namespace Aldebaran.Application.Services.Models
{
    public class Forwarder
    {
        public int ForwarderId { get; set; }
        public string ForwarderName { get; set; } = null!;
        public string Phone1 { get; set; } = null!;
        public string Phone2 { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string ForwarderAddress { get; set; } = null!;
        public string Mail1 { get; set; } = null!;
        public string Mail2 { get; set; } = null!;
        public int CityId { get; set; }
        // Reverse navigation
        public ICollection<ForwarderAgent> ForwarderAgents { get; set; }
        public City City { get; set; } = null!;
        public Forwarder()
        {
            ForwarderAgents = new List<ForwarderAgent>();
        }
    }
}
