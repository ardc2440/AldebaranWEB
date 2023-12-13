namespace Aldebaran.Application.Services.Models
{
    public class Provider
    {
        public int ProviderId { get; set; }
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderAddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public int CityId { get; set; }
        // Reverse navigation
        public ICollection<ProviderReference> ProviderReferences { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public City City { get; set; }
        public IdentityType IdentityType { get; set; }
        public Provider()
        {
            ProviderReferences = new List<ProviderReference>();
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
