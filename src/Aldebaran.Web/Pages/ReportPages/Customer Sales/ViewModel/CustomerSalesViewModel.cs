namespace Aldebaran.Web.Pages.ReportPages.Customer_Sales.ViewModel
{
    public class CustomerSalesViewModel
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
            public List<Reference> References { get; set; }
        }
        public class Reference
        {
            public string ItemReference { get; set; }
            public string ItemName { get; set; }
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int Amount { get; set; }
            public int DeliveredAmount { get; set; }
            public List<Shipment> Shipments { get; set; }
        }
        public class Shipment
        {
            public DateTime ShipmentDate { get; set; }
            public string DeliveryNote { get; set; }
            public string TrackingNumber { get; set; }
            public string ShipmentMethodName { get; set; }
            public string Notes { get; set; }
            public List<ShipmentReference> References { get; set; }
        }
        public class ShipmentReference
        {
            public string ItemReference { get; set; }
            public string ItemName { get; set; }
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public int Amount { get; set; }
        }
    }
}
