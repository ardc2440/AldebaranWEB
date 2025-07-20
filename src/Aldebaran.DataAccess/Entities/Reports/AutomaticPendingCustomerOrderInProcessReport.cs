namespace Aldebaran.DataAccess.Entities.Reports
{
    public class AutomaticPendingCustomerOrderInProcessReport
    {       
        public int CustomerOrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string StatusOrderName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerIdentity { get; set; }
        public string CustomerOrderEmployeeName { get; set; }        
        
        public int CustomerOrderInProcessId { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime TransferDatetime { get; set; }
        public string Notes { get; set; }        
        
        public string ItemName { get; set; }
        public string InternalReference { get; set; }
        public string ReferenceName { get; set; }
        public string Brand { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }        
    }
}
