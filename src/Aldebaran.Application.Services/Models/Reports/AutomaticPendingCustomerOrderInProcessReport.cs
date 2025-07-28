namespace Aldebaran.Application.Services.Models.Reports
{
    public class AutomaticPendingCustomerOrderInProcessReport
    {       
        public int CustomerOrderId { get; set; }
        public required string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public required string StatusOrderName { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerIdentity { get; set; }
        public required string CustomerOrderEmployeeName { get; set; }        
        
        public int CustomerOrderInProcessId { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime TransferDatetime { get; set; }
        public string? Notes { get; set; }        
        
        public required string ItemName { get; set; }
        public required string InternalReference { get; set; }
        public required string ReferenceName { get; set; }
        public string? Brand { get; set; }
        public required string WarehouseName { get; set; }
        public int Quantity { get; set; }        
    }
}
