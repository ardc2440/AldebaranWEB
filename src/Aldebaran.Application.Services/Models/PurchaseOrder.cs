namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public DateTime ExpectedReceiptDate { get; set; }
        public DateTime? RealReceiptDate { get; set; }
        public int ProviderId { get; set; }
        public int? ForwarderAgentId { get; set; }
        public short? ShipmentForwarderAgentMethodId { get; set; }
        public int EmployeeId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public string? ImportNumber { get; set; }
        public string EmbarkationPort { get; set; } = null!;
        public string ProformaNumber { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        // Reverse navigation
        public ICollection<ModifiedPurchaseOrder> ModifiedPurchaseOrders { get; set; }
        public ICollection<PurchaseOrderActivity> PurchaseOrderActivities { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public Employee Employee { get; set; } = null!;
        public ForwarderAgent? ForwarderAgent { get; set; }
        public Provider Provider { get; set; } = null!;
        public ShipmentForwarderAgentMethod? ShipmentForwarderAgentMethod { get; set; }
        public StatusDocumentType? StatusDocumentType { get; set; }
        public PurchaseOrder()
        {
            EmbarkationPort = " ";
            ProformaNumber = " ";
            CreationDate = DateTime.Now;
            ModifiedPurchaseOrders = new List<ModifiedPurchaseOrder>();
            PurchaseOrderActivities = new List<PurchaseOrderActivity>();
            PurchaseOrderDetails = new List<PurchaseOrderDetail>();
        }

    }
}
