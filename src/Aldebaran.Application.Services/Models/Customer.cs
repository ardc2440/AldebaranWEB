namespace Aldebaran.Application.Services.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; }
        public string CustomerName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string CustomerAddress { get; set; }
        public string CellPhone { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public int CityId { get; set; }
        public string Email3 { get; set; }
        // Reverse navigation
        public ICollection<CustomerContact> CustomerContacts { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public City City { get; set; }
        public IdentityType IdentityType { get; set; }
        public Customer()
        {
            CustomerContacts = new List<CustomerContact>();
            CustomerOrders = new List<CustomerOrder>();
            CustomerReservations = new List<CustomerReservation>();
        }

    }
}
