using Aldebaran.DataAccess.Core;
using Aldebaran.DataAccess.Core.Atributes;

namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrder : ITrackeable
    {
        public int PurchaseOrderId { get; set; }
        [Sequence(length: 10)]
        public string OrderNumber { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public DateTime ExpectedReceiptDate { get; set; }
        public DateTime? RealReceiptDate { get; set; }
        public int ProviderId { get; set; }
        public int? ForwarderAgentId { get; set; }
        public short? ShipmentForwarderAgentMethodId { get; set; }
        public int EmployeeId { get; set; }
        public short StatusDocumentTypeId { get; set; }
        public string ImportNumber { get; set; } = null!;
        public string EmbarkationPort { get; set; }
        public string ProformaNumber { get; set; }
        public DateTime CreationDate { get; set; }
        // Reverse navigation
        public CanceledPurchaseOrder CanceledPurchaseOrder { get; set; } = null!;
        public ICollection<ModifiedPurchaseOrder> ModifiedPurchaseOrders { get; set; }
        public ICollection<PurchaseOrderActivity> PurchaseOrderActivities { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public Employee Employee { get; set; } = null!;
        public ForwarderAgent ForwarderAgent { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
        public ShipmentForwarderAgentMethod ShipmentForwarderAgentMethod { get; set; } = null!;
        public StatusDocumentType StatusDocumentType { get; set; } = null!;
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
