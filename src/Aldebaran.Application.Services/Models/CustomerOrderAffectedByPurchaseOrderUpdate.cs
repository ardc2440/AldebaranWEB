namespace Aldebaran.Application.Services.Models
{
    public class CustomerOrderAffectedByPurchaseOrderUpdate
    {
        public int CustomerOrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
