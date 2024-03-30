using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;
namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel
{
    public class CustomerOrderFilter : ICloneable
    {
        public string OrderNumber { get; set; }
        public DateRange CreationDate { get; set; } = new();
        public DateRange OrderDate { get; set; } = new();
        public DateRange EstimatedDeliveryDate { get; set; } = new();
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
