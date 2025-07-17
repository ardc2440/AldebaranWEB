namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel
{
    public class PendingCustomerOrderInProcessViewModel
    {
        public List<CustomerOrder> CustomerOrders { get; set; }

        public class CustomerOrder
        {
            public int CustomerOrderId { get; set; }
            public string OrderNumber { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime EstimatedDeliveryDate { get; set; }
            public string StatusOrderName { get; set; }
            public string CustomerName { get; set; }
            public string CustomerIdentity { get; set; }
            public string EmployeeName { get; set; }
            public List<CustomerOrderInProcess> CustomerOrdersInProcess { get; set; }
        }

        public class CustomerOrderInProcess
        {
            public int CustomerOrderInProcessId { get; set; }
            public DateTime ProcessDate { get; set; }
            public DateTime TransferDatetime { get; set; }
            public string StatusName { get; set; }
            public string ProcessSatelliteName { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeRecipientName { get; set; }
            public string Notes { get; set; }
            public List<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
        }

        public class CustomerOrderInProcessDetail
        {
            public int CustomerOrderInProcessDetailId { get; set; }
            public string ItemName { get; set; }
            public string InternalReference { get; set; }
            public string ReferenceName { get; set; }
            public string Brand { get; set; }
            public string WarehouseName { get; set; }
            public int Quantity { get; set; }            
        }
    }
}