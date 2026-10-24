namespace Aldebaran.Application.Services.Models
{
    public class CustomerContact
    {
        public int CustomerContactId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerContactName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

    }
}
