using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel
{
    public class PendingCustomerOrderInProcessFilter : ICloneable
    {
        public string OrderNumber { get; set; }
        public DateRange OrderDate { get; set; } = new();
        public DateRange ProcessDate { get; set; } = new();
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}