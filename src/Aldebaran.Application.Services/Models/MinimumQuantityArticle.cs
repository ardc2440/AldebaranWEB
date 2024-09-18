namespace Aldebaran.Application.Services.Models
{
    public class MinimumQuantityArticle
    {
        public int AlarmId { get; set; }
        public int ReferenceId { get; set; }
        public required string ArticleName { get; set; }
        public int MinimumQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int InTransitQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int OrderedQuantity { get; set; }               
    }
}
