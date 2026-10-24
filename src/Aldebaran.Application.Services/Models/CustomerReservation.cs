namespace Aldebaran.Application.Services.Models
{
    public class CustomerReservation
    {
        public int CustomerReservationId { get; set; }
        public int CustomerId { get; set; }
        public string ReservationNumber { get; set; } = null!;
        public DateTime ReservationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; } = null!;
        public int EmployeeId { get; set; }
        public int? CustomerOrderId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        // Reverse navigation
        public CanceledCustomerReservation CanceledCustomerReservation { get; set; } = null!;
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }
        public ICollection<ModifiedCustomerReservation> ModifiedCustomerReservations { get; set; }

        public ICollection<CustomerReservationNotification> CustomerReservationNotifications { get; set; }
        public Customer Customer { get; set; } = null!;
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;
        public CustomerReservation()
        {
            CreationDate = DateTime.Now;
            CustomerReservationDetails = new List<CustomerReservationDetail>();
            ModifiedCustomerReservations = new List<ModifiedCustomerReservation>();
            CustomerReservationNotifications = new List<CustomerReservationNotification>();
        }
    }
}
