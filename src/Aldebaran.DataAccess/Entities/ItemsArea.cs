using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class ItemsArea : ITrackeable
    {
        public int ItemId { get; set; }
        public short AreaId { get; set; }
        public Area Area { get; set; } = null!;
        public Item Item { get; set; } = null!;
    }
}
