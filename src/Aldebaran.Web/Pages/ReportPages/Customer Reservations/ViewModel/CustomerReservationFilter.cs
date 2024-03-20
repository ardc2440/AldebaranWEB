using Aldebaran.Application.Services.Models;
namespace Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel
{
    public class CustomerReservationFilter : ICloneable
    {
        public string ReservationNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ReservationDate { get; set; }
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
