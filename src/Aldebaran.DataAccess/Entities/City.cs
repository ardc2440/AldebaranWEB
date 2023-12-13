namespace Aldebaran.DataAccess.Entities
{
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int DepartmentId { get; set; }
        // Reverse navigation
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Forwarder> Forwarders { get; set; }
        public ICollection<ForwarderAgent> ForwarderAgents { get; set; }
        public ICollection<ProcessSatellite> ProcessSatellites { get; set; }
        public ICollection<Provider> Providers { get; set; }
        public Department Department { get; set; }
        public City()
        {
            Customers = new List<Customer>();
            ForwarderAgents = new List<ForwarderAgent>();
            Forwarders = new List<Forwarder>();
            ProcessSatellites = new List<ProcessSatellite>();
            Providers = new List<Provider>();
        }
    }
}
