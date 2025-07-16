namespace Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Order_In_Process_Creation.ViewModel
{
    public class BackOrderViewModel
    {
        public List<Customer> Customers { get; set; }
        public class Customer
        {
            public string CustomerName { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public List<Order> Orders { get; set; }
        }
        public class Order
        {
            public string OrderNumber { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime EstimatedDeliveryDate { get; set; }
            public string Status { get; set; }
            public string InternalNotes { get; set; }
            public string CustomerNotes { get; set; }
            public List<Reference> References { get; set; }
        }
        public class Reference
        {
            public string ItemReference { get; set; }
            public string ItemName { get; set; }
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int PendingAmount { get; set; }
            public string Status { get; set; }
        }        
    }
}
