namespace Aldebaran.DataAccess.Entities
{
    public class Line
    {
        public short LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public bool IsDemon { get; set; }
        public bool IsActive { get; set; }
        // Reverse navigation
        public ICollection<Item> Items { get; set; }
        public Line()
        {
            IsDemon = false;
            IsActive = true;
            Items = new List<Item>();
        }
    }
}
