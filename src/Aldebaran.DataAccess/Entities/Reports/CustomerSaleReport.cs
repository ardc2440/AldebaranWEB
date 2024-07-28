using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities.Reports
{
    public class CustomerSaleReport
    {
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }

        public int? CustomerOrderId { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string? Status { get; set; }
        public string? InternalNotes { get; set; }

        public int OrderDetailId { get; set; }
        public string? ItemReference { get; set; }
        public string? ItemName { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceName { get; set; }
        public int? Amount { get; set; }
        public int? DeliveredAmount { get; set; }

        public int? ShipmentId { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string? DeliveryNote { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShipmentMethodName { get; set; }
        public string? Notes { get; set; }

        public int? ShipmentDetailId { get; set; }
        public string? ShipmentItemReference { get; set; }
        public string? ShipmentItemName { get; set; }
        public string? ShipmentReferenceCode { get; set; }
        public string? ShipmentReferenceName { get; set; }
        public int? ShipmentAmount { get; set; }
    }
}
