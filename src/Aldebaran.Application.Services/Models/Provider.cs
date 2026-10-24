namespace Aldebaran.Application.Services.Models
{
    public class Provider
    {
        public int ProviderId { get; set; }
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string ProviderCode { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string ProviderAddress { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;
        public int CityId { get; set; }
        // Reverse navigation
        public ICollection<ProviderReference> ProviderReferences { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public City City { get; set; } = null!;
        public IdentityType IdentityType { get; set; } = null!;
        public Provider()
        {
            ProviderReferences = new List<ProviderReference>();
            PurchaseOrders = new List<PurchaseOrder>();
        }
    }
}
