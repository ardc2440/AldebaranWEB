using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class Packaging : ITrackeable
    {
        public short PackagingId { get; set; }
        public int ItemId { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Length { get; set; }
        public int? Quantity { get; set; }
        public Item Item { get; set; }
    }
}
