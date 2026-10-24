namespace Aldebaran.DataAccess.Entities.Reports
{
    public class AutomaticPendingCustomerOrderInProcessReport
    {       
        public int CustomerOrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public bool IsSpecialOrder { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string StatusOrderName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerIdentity { get; set; } = null!;
        public string CustomerOrderEmployeeName { get; set; } = null!;        
        
        public int CustomerOrderInProcessId { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime TransferDatetime { get; set; }
        public string Notes { get; set; } = null!;        
        
        public string ItemName { get; set; } = null!;
        public string InternalReference { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public int Quantity { get; set; }        
    }
}
