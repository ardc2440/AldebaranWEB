using Aldebaran.DataAccess.Core;
using Aldebaran.DataAccess.Core.Atributes;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrder : ITrackeable
    {
        public int CustomerOrderId { get; set; }
        public int CustomerId { get; set; }
        [Sequence(length: 10)]
        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string InternalNotes { get; set; } = null!;
        public int EmployeeId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CustomerNotes { get; set; } = null!;
        public bool IsSpecialOrder { get; set; }
        // Reverse navigation
        public CanceledCustomerOrder CanceledCustomerOrder { get; set; } = null!;
        public ICollection<ClosedCustomerOrder> ClosedCustomerOrders { get; set; }
        public ICollection<CustomerOrderActivity> CustomerOrderActivities { get; set; }
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public ICollection<ModifiedCustomerOrder> ModifiedCustomerOrders { get; set; }
        public ICollection<PurchaseOrderNotification> PurchaseOrderNotifications { get; set; } = null!;
        public ICollection<CustomerOrderNotification> CustomerOrderNotifications { get; set; } = null!;

        public Customer Customer { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;
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
