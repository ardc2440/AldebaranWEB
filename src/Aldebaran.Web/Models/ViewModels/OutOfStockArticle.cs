namespace Aldebaran.Web.Models.ViewModels
{
    public class OutOfStockArticle
    {
        public string ArticleName { get; set; }
        public int AvailableQuantity { get; set; }
        public int InTransitQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int OrderedQuantity { get; set; }

        public async static Task<IEnumerable<OutOfStockArticle>> GetOutOfStockArticleListAsync(IEnumerable<Application.Services.Models.ItemReference> outOfStockReferences, IEnumerable<Application.Services.Models.PurchaseOrderDetail> referencesInTransit)
        {
            var result = new List<OutOfStockArticle>();

            foreach (var outOfStockReference in outOfStockReferences)
            {
                var outOfStockArticle = new OutOfStockArticle
                {
                    ArticleName = $"({outOfStockReference.Item.Line.LineName}) {outOfStockReference.Item.ItemName} - {outOfStockReference.ReferenceName}",
                    AvailableQuantity = outOfStockReference.InventoryQuantity,
                    InTransitQuantity = referencesInTransit.Where(i => i.ReferenceId == outOfStockReference.ReferenceId).Sum(i => i.RequestedQuantity),
                    ReservedQuantity = outOfStockReference.ReservedQuantity,
                    OrderedQuantity = outOfStockReference.OrderedQuantity,                    
                };

                result.Add(outOfStockArticle);
            }

            return result.Where(i => (i.AvailableQuantity + i.InTransitQuantity) <= 0);
        }
    }
}
