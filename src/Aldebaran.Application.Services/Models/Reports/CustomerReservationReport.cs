namespace Aldebaran.Application.Services.Models.Reports
{
    public class CustomerReservationReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public int ReservationId { get; set; }
        public string ReservationNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public int Amount { get; set; }
    }
}
