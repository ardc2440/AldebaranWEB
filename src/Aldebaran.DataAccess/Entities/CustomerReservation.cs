using Aldebaran.DataAccess.Core.Atributes;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerReservation
    {
        public int CustomerReservationId { get; set; }
        public int CustomerId { get; set; }
        [Sequence(length: 10)]
        public string ReservationNumber { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public int EmployeeId { get; set; }
        public int? CustomerOrderId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        // Reverse navigation
        public CanceledCustomerReservation CanceledCustomerReservation { get; set; }
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }
        public ICollection<ModifiedCustomerReservation> ModifiedCustomerReservations { get; set; }
        public Customer Customer { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public Employee Employee { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public CustomerReservation()
        {
            CreationDate = DateTime.Now;
            CustomerReservationDetails = new List<CustomerReservationDetail>();
            ModifiedCustomerReservations = new List<ModifiedCustomerReservation>();
        }
    }
}
