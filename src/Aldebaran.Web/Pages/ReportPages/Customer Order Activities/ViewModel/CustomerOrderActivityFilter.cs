using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.ViewModel
{
    public class CustomerOrderActivityFilter : ICloneable
    {
        public string OrderNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; } = DateTime.Now;
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; } = DateTime.Now;
        public DateTime? EstimatedDeliveryDateFrom { get; set; }
        public DateTime? EstimatedDeliveryDateTo { get; set; } = DateTime.Now;
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
