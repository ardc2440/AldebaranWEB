namespace Aldebaran.DataAccess.Entities
{
    public class ProcessSatellite
    {
        public int ProcessSatelliteId { get; set; }
        public string ProcessSatelliteName { get; set; }
        public string ProcessSatelliteAddress { get; set; }
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }
        public string LegalRepresentative { get; set; }
        public bool IsActive { get; set; }
        // Reverse navigation
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public City City { get; set; }
        public IdentityType IdentityType { get; set; }
        public ProcessSatellite()
        {
            IsActive = true;
            CustomerOrdersInProcesses = new List<CustomerOrdersInProcess>();
        }
    }
}
