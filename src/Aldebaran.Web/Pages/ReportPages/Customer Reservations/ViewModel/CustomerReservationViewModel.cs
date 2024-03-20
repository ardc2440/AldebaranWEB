namespace Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel
{
    public class CustomerReservationViewModel
    {
        public List<Customer> Customers { get; set; }
        public class Customer
        {
            public string CustomerName { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public List<Reservation> Reservations { get; set; }
            public int Total => Reservations?.Sum(w => w.Total) ?? 0;
        }
        public class Reservation
        {
            public string ReservationNumber { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime ReservationDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public string Status { get; set; }
            public string Notes { get; set; }
            public List<Item> Items { get; set; }
            public int Total => Items?.SelectMany(w => w.References).Sum(w => w.Amount) ?? 0;
        }

        public class Item
        {
            public string InternalReference { get; set; }
            public string ItemName { get; set; }
            public List<Reference> References { get; set; }

        }
        public class Reference
        {
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int Amount { get; set; }
            public decimal Price { get; set; }

        }
    }
}
