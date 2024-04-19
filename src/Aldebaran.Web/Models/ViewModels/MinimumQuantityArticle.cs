namespace Aldebaran.Web.Models.ViewModels
{
    public class MinimumQuantityArticle
    {
        public string ArticleName { get; set; }
        public int MinimumQuantity { get; set;}        
        public int AvailableQuantity { get; set;}
        public int InTransitQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int OrderedQuantity { get; set; }

        public async static Task<IEnumerable<MinimumQuantityArticle>> GetMinimuQuantityArticleListAsync(IEnumerable<Application.Services.Models.ItemReference> minimumQuantityReferences, IEnumerable<Application.Services.Models.PurchaseOrderDetail> referencesInTransit)
        {
            var result = new List<MinimumQuantityArticle>();

            foreach (var minimumQuantityReference in minimumQuantityReferences)
            {
                var minimumQuantityArticle = new MinimumQuantityArticle
                {
                    ArticleName = $"({minimumQuantityReference.Item.Line.LineName}) {minimumQuantityReference.Item.ItemName} - {minimumQuantityReference.ReferenceName}",
                    AvailableQuantity = minimumQuantityReference.InventoryQuantity,
                    MinimumQuantity = minimumQuantityReference.AlarmMinimumQuantity,
                    InTransitQuantity = referencesInTransit.Where(i=>i.ReferenceId==minimumQuantityReference.ReferenceId).Sum(i => i.RequestedQuantity),
                    ReservedQuantity = minimumQuantityReference.ReservedQuantity,
                    OrderedQuantity = minimumQuantityReference.OrderedQuantity,
                };

                result.Add(minimumQuantityArticle); 
            }

            return result.Where(i=>i.MinimumQuantity>=(i.AvailableQuantity+i.InTransitQuantity));
        }
    }
}
