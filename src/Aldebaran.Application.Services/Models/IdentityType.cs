namespace Aldebaran.Application.Services.Models
{
    public class IdentityType
    {
        public int IdentityTypeId { get; set; }
        public string IdentityTypeCode { get; set; }
        public string IdentityTypeName { get; set; }
        // Reverse navigation
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<ProcessSatellite> ProcessSatellites { get; set; }
        public ICollection<Provider> Providers { get; set; }
        public IdentityType()
        {
            Customers = new List<Customer>();
            Employees = new List<Employee>();
            ProcessSatellites = new List<ProcessSatellite>();
            Providers = new List<Provider>();
        }
    }
}
