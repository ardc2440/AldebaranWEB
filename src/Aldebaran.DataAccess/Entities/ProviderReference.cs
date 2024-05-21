using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class ProviderReference : ITrackeable
    {
        public int ReferenceId { get; set; }
        public int ProviderId { get; set; }
        public ItemReference ItemReference { get; set; }
        public Provider Provider { get; set; }
    }
}
