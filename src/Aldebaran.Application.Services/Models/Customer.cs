namespace Aldebaran.Application.Services.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string Phone1 { get; set; } = null!;
        public string Phone2 { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string CustomerAddress { get; set; } = null!;
        public string CellPhone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int CityId { get; set; }
        // Reverse navigation
        public ICollection<CustomerContact> CustomerContacts { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public City City { get; set; } = null!;
        public IdentityType IdentityType { get; set; } = null!;
        public Customer()
        {
            CustomerContacts = new List<CustomerContact>();
            CustomerOrders = new List<CustomerOrder>();
            CustomerReservations = new List<CustomerReservation>();
        }

    }
}
