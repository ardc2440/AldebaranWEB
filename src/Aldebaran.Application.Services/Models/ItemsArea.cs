namespace Aldebaran.Application.Services.Models
{
    public class ItemsArea
    {
        public int ItemId { get; set; }
        public short AreaId { get; set; }
        public Area Area { get; set; }
        public Item Item { get; set; }
    }
}
