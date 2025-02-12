namespace Aldebaran.Application.Services.Models
{
    public class AutomaticCustomerOrder
    {
        public int OrderId { get; set; }
        public int CustomerOrderId { get; set; }
        public required string OrderNumber { get; set; }
        public required string IdentityNumber { get; set; }
        public required string CustomerName { get; set; }  
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public required string StatusDocumentTypeName { get; set; }
    }
}
