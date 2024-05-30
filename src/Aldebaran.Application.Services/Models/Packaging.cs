namespace Aldebaran.Application.Services.Models
{
    public class Packaging
    {
        public short PackagingId { get; set; }
        public int ItemId { get; set; }
        public double? Weight { get; set; } = 0;
        public double? Height { get; set; } = 0;
        public double? Width { get; set; } = 0;
        public double? Length { get; set; } = 0;
        public int? Quantity { get; set; } = 0;
        public Item Item { get; set; } 
    }
}
