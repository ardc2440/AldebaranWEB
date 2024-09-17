namespace Aldebaran.Application.Services.Models
{
    public class OutOfStockArticle
    {
        public int ReferenceId { get; set; }
        public required string ArticleName { get; set; }
        public int AvailableQuantity { get; set; }
        public int InTransitQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int OrderedQuantity { get; set; }        
    }
}
