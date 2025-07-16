namespace Aldebaran.Application.Services.Models.Reports
{
    public class BackOrderReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderCreationDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public string InternalNotes { get; set; }
        public string CustomerNotes { get; set; }

        public int OrderDetailId { get; set; }
        public string OrderDetailItemReference { get; set; }
        public string OrderDetailItemName { get; set; }
        public string OrderDetailReferenceCode { get; set; }
        public string OrderDetailReferenceName { get; set; }
        public int OrderDetailPendingAmount { get; set; }
        public string DetailStatus { get; set; }        
    }
}
