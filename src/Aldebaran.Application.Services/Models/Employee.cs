namespace Aldebaran.Application.Services.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public short AreaId { get; set; }
        public int IdentityTypeId { get; set; }
        public string IdentityNumber { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string LoginUserId { get; set; }
        public string Position { get; set; }
        // Reverse navigation
        public ICollection<Adjustment> Adjustments { get; set; }
        public ICollection<CanceledCustomerOrder> CanceledCustomerOrders { get; set; }
        public ICollection<CanceledCustomerReservation> CanceledCustomerReservations { get; set; }
        public ICollection<CanceledOrderShipment> CanceledOrderShipments { get; set; }
        public ICollection<CanceledOrdersInProcess> CanceledOrdersInProcesses { get; set; }
        public ICollection<CanceledPurchaseOrder> CanceledPurchaseOrders { get; set; }
        public ICollection<ClosedCustomerOrder> ClosedCustomerOrders { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<CustomerOrderActivity> CustomerOrderActivities { get; set; }
        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails_ActivityEmployeeId { get; set; }
        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails_EmployeeId { get; set; }
        public ICollection<CustomerOrderShipment> CustomerOrderShipments { get; set; }
        public ICollection<CustomerOrdersInProcess> CustomerOrdersInProcesses { get; set; }
        public ICollection<CustomerReservation> CustomerReservations { get; set; }
        public ICollection<ModifiedCustomerOrder> ModifiedCustomerOrders { get; set; }
        public ICollection<ModifiedCustomerReservation> ModifiedCustomerReservations { get; set; }
        public ICollection<ModifiedOrderShipment> ModifiedOrderShipments { get; set; }
        public ICollection<ModifiedOrdersInProcess> ModifiedOrdersInProcesses { get; set; }
        public ICollection<ModifiedPurchaseOrder> ModifiedPurchaseOrders { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<PurchaseOrderActivity> PurchaseOrderActivities_ActivityEmployeeId { get; set; }
        public ICollection<PurchaseOrderActivity> PurchaseOrderActivities_EmployeeId { get; set; }
        public ICollection<UsersAlarmType> UsersAlarmTypes { get; set; }
        public ICollection<VisualizedAlarm> VisualizedAlarms { get; set; }
        public ICollection<VisualizedPurchaseOrderTransitAlarm> VisualizedPurchaseOrderTransitAlarms { get; set; }
        public ICollection<VisualizedOutOfStockInventoryAlarm> VisualizedOutOfStockInventoryAlarms { get; set; }
        public ICollection<VisualizedMinimumQuantityAlarm> VisualizedMinimumQuantityAlarms { get; set; }
        public ICollection<VisualizedMinimumLocalWarehouseQuantityAlarm> VisualizedMinimumLocalWarehouseQuantityAlarms { get; set; }
        public ICollection<VisualizedAutomaticInProcess> VisualizedAutomaticInProcesses { get; set; }

        public ICollection<WarehouseTransfer> WarehouseTransfers { get; set; }
        public ICollection<CancellationRequest> RequestEmployees { get; set; }
        public ICollection<CancellationRequest> ResponseEmployees { get; set; }
        public Area Area { get; set; }
        public IdentityType IdentityType { get; set; }
        public Employee()
        {
            Adjustments = new List<Adjustment>();
            CanceledCustomerOrders = new List<CanceledCustomerOrder>();
            CanceledCustomerReservations = new List<CanceledCustomerReservation>();
            CanceledOrderShipments = new List<CanceledOrderShipment>();
            CanceledOrdersInProcesses = new List<CanceledOrdersInProcess>();
            CanceledPurchaseOrders = new List<CanceledPurchaseOrder>();
            ClosedCustomerOrders = new List<ClosedCustomerOrder>();
            CustomerOrderActivities = new List<CustomerOrderActivity>();
            CustomerOrderActivityDetails_ActivityEmployeeId = new List<CustomerOrderActivityDetail>();
            CustomerOrderActivityDetails_EmployeeId = new List<CustomerOrderActivityDetail>();
            CustomerOrderShipments = new List<CustomerOrderShipment>();
            CustomerOrders = new List<CustomerOrder>();
            CustomerOrdersInProcesses = new List<CustomerOrdersInProcess>();
            CustomerReservations = new List<CustomerReservation>();
            ModifiedCustomerOrders = new List<ModifiedCustomerOrder>();
            ModifiedCustomerReservations = new List<ModifiedCustomerReservation>();
            ModifiedOrderShipments = new List<ModifiedOrderShipment>();
            ModifiedOrdersInProcesses = new List<ModifiedOrdersInProcess>();
            ModifiedPurchaseOrders = new List<ModifiedPurchaseOrder>();
            PurchaseOrderActivities_ActivityEmployeeId = new List<PurchaseOrderActivity>();
            PurchaseOrderActivities_EmployeeId = new List<PurchaseOrderActivity>();
            PurchaseOrders = new List<PurchaseOrder>();
            UsersAlarmTypes = new List<UsersAlarmType>();
            VisualizedAlarms = new List<VisualizedAlarm>();
            VisualizedPurchaseOrderTransitAlarms = new List<VisualizedPurchaseOrderTransitAlarm>();
            VisualizedOutOfStockInventoryAlarms = new List<VisualizedOutOfStockInventoryAlarm>();
            VisualizedMinimumQuantityAlarms = new List<VisualizedMinimumQuantityAlarm>();
            VisualizedMinimumLocalWarehouseQuantityAlarms = new List<VisualizedMinimumLocalWarehouseQuantityAlarm>();
        }

    }
}
