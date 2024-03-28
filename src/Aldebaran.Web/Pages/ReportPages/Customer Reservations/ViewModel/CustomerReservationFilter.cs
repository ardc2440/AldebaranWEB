using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel
{
    public class CustomerReservationFilter : ICloneable
    {
        public string ReservationNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; } = DateTime.Now;
        public DateTime? ReservationDateFrom { get; set; }
        public DateTime? ReservationDateTo { get; set; } = DateTime.Now;
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
