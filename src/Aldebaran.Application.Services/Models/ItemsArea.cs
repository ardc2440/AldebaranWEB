namespace Aldebaran.Application.Services.Models
{
    public class ItemsArea
    {
        public int ItemId { get; set; }
        public short AreaId { get; set; }
        public Area Area { get; set; } = null!;
        public Item Item { get; set; } = null!;
    }
}
