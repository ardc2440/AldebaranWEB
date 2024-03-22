using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel
{
    public class CustomerOrderFilter : ICloneable
    {
        public string OrderNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
