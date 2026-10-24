namespace Aldebaran.DataAccess.Entities.Reports
{
    public class CustomerOrderReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Fax { get; set; }

        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public bool IsSpecialOrder { get; set; }
        public DateTime OrderCreationDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string OrderStatus { get; set; } = null!;
        public string? InternalNotes { get; set; }
        public string? CustomerNotes { get; set; }

        public int OrderDetailId { get; set; }
        public string OrderDetailItemReference { get; set; } = null!;
        public string OrderDetailItemName { get; set; } = null!;
        public string OrderDetailReferenceCode { get; set; } = null!;
        public string OrderDetailReferenceName { get; set; } = null!;
        public int OrderDetailAmount { get; set; }
        public int DeliveredAmount { get; set; }
        public int InProcessAmount { get; set; }
        public string DetailStatus { get; set; } = null!;

        public int? ShipmentId { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string? DeliveryNote { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShipmentMethodName { get; set; }
        public string? Notes { get; set; }

        public int? ShipmentDetailId { get; set; }
        public string? ShipmentDetailItemReference { get; set; }
        public string? ShipmentDetailItemName { get; set; }
        public string? ShipmentDetailReferenceCode { get; set; }
        public string? ShipmentDetailReferenceName { get; set; }
        public int? ShipmentDetailAmount { get; set; }
    }
}
