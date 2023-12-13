namespace Aldebaran.DataAccess.Entities
{
    public class CustomerContact
    {
        public int CustomerContactId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerContactName { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Customer Customer { get; set; }
    }
}
