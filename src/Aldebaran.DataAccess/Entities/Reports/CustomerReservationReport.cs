namespace Aldebaran.DataAccess.Entities.Reports
{
    public class CustomerReservationReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Fax { get; set; }

        public int ReservationId { get; set; }
        public string ReservationNumber { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;

        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public int Amount { get; set; }        
    }
}
