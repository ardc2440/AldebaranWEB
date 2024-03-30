namespace Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.ViewModel
{
    public class CustomerOrderActivityViewModel
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
            public List<Activity> Activities { get; set; }
        }
        public class Reference
        {
            public string ItemReference { get; set; }
            public string ItemName { get; set; }
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int Amount { get; set; }
            public int DeliveredAmount { get; set; }
            public int InProcessAmount { get; set; }
            public string Status { get; set; }
        }
        public class Activity
        {
            public DateTime CreationDate { get; set; }
            public string AreaName { get; set; }
            public string EmployeeName { get; set; }
            public string Notes { get; set; }
            public List<ActivityDetail> Details { get; set; }
        }
        public class ActivityDetail
        {
            public string ActivityTypeName { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}
