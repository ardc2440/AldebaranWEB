namespace Aldebaran.DataAccess.Entities
{
    public class ProcessSatellite
    {
        public int ProcessSatelliteId { get; set; }
        public string ProcessSatelliteName { get; set; } = null!;
        public string ProcessSatelliteAddress { get; set; } = null!;
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int CityId { get; set; }
        public string LegalRepresentative { get; set; } = null!;
        public bool IsActive { get; set; }
        // Reverse navigation
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public City City { get; set; } = null!;
        public IdentityType IdentityType { get; set; } = null!;
        public ProcessSatellite()
        {
            IsActive = true;
            CustomerOrdersInProcesses = new List<CustomerOrdersInProcess>();
        }
    }
}
