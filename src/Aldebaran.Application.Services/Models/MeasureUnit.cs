namespace Aldebaran.Application.Services.Models
{
    public class MeasureUnit
    {
        public short MeasureUnitId { get; set; }
        public string MeasureUnitName { get; set; }
        // Reverse navigation
        public ICollection<Item> Items_CifMeasureUnitId { get; set; }
        public ICollection<Item> Items_FobMeasureUnitId { get; set; }
        public MeasureUnit()
        {
            Items_CifMeasureUnitId = new List<Item>();
            Items_FobMeasureUnitId = new List<Item>();
        }
    }
}
