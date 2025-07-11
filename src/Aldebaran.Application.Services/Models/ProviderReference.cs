namespace Aldebaran.Application.Services.Models
{
    public class ProviderReference
    {
        public int ReferenceId { get; set; }
        public int ProviderId { get; set; }
        public ItemReference ItemReference { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
    }
}
