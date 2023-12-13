namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrder
    {
        public int CustomerOrderId { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string InternalNotes { get; set; }
        public int EmployeeId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CustomerNotes { get; set; }
        // Reverse navigation
        public CanceledCustomerOrder CanceledCustomerOrder { get; set; }
        public ICollection<ClosedCustomerOrder> ClosedCustomerOrders { get; set; }
        public ICollection<CustomerOrderActivity> CustomerOrderActivities { get; set; }
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public ICollection<ModifiedCustomerOrder> ModifiedCustomerOrders { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public StatusDocumentType StatusDocumentType { get; set; }
        public CustomerOrder()
        {
            CreationDate = DateTime.Now;
            ClosedCustomerOrders = new List<ClosedCustomerOrder>();
            CustomerOrderActivities = new List<CustomerOrderActivity>();
            CustomerOrderDetails = new List<CustomerOrderDetail>();
            CustomerOrderShipments = new List<CustomerOrderShipment>();
            CustomerOrdersInProcesses = new List<CustomerOrdersInProcess>();
            CustomerReservations = new List<CustomerReservation>();
            ModifiedCustomerOrders = new List<ModifiedCustomerOrder>();
        }
    }
}
