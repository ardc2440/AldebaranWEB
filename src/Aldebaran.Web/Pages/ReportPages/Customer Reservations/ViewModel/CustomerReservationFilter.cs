using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;
namespace Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel
{
    public class CustomerReservationFilter : ICloneable
    {
        public string ReservationNumber { get; set; }
        public DateRange CreationDate { get; set; } = new();
        public DateRange ReservationDate { get; set; } = new();
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
