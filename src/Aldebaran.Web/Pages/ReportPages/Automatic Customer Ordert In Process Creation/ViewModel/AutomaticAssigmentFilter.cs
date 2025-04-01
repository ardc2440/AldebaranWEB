using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;

namespace Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Ordert_In_Process_Creation.ViewModel
{
    public class AutomaticAssigmentFilter : ICloneable
    {
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public int? ProviderId { get; set; }
        public Provider Provider { get; set; }
        public string ProformaNumber { get; set; }
        public string ImportNumber { get; set; }
        public DateRange ReceiptDate { get; set; } = new();
        public DateRange ConfirmedDate { get; set; } = new();
        public string CustomerOrderNumber { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateRange OrderDate { get; set; } = new();
        public DateRange EstimatedDeliveryDate { get; set; } = new();
        public short? StatusDocumentTypeId { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
