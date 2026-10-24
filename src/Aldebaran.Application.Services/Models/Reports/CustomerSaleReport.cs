namespace Aldebaran.Application.Services.Models.Reports
{
    public class CustomerSaleReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;

        public int CustomerOrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public bool IsSpecialOrder { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string Status { get; set; } = null!;
        public string InternalNotes { get; set; } = null!;

        public int OrderDetailId { get; set; }
        public string ItemReference { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public string ReferenceName { get; set; } = null!;
        public int Amount { get; set; }
        public int DeliveredAmount { get; set; }

        public int ShipmentId { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string DeliveryNote { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public string ShipmentMethodName { get; set; } = null!;
        public string Notes { get; set; } = null!;

        public int ShipmentDetailId { get; set; }
        public string ShipmentItemReference { get; set; } = null!;
        public string ShipmentItemName { get; set; } = null!;
        public string ShipmentReferenceCode { get; set; } = null!;
        public string ShipmentReferenceName { get; set; } = null!;
        public int ShipmentAmount { get; set; }
    }
}
