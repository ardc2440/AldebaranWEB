using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("purchase_orders", Schema = "dbo")]
    public partial class PurchaseOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PURCHASE_ORDER_ID { get; set; }

        [Required]
        public string ORDER_NUMBER { get; set; }

        [Required]
        public DateTime REQUEST_DATE { get; set; }

        [Required]
        public DateTime EXPECTED_RECEIPT_DATE { get; set; }

        public DateTime? REAL_RECEIPT_DATE { get; set; }

        [Required]
        public int PROVIDER_ID { get; set; }

        public int? FORWARDER_AGENT_ID { get; set; }

        public short? SHIPMENT_FORWARDER_AGENT_METHOD_ID { get; set; }

        [Required]
        public int USER_ID { get; set; }

        [Required]
        public short STATUS_DOCUMENT_TYPE_ID { get; set; }

        public string IMPORT_NUMBER { get; set; }

        public string EMBARKATION_PORT { get; set; }

        public string PROFORMA_NUMBER { get; set; }

        public DateTime CREATION_DATE { get; set; }

        public ICollection<PurchaseOrderActivity> PurchaseOrderActivities { get; set; }

        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public ForwarderAgent ForwarderAgent { get; set; }

        public Provider Provider { get; set; }

        public ShipmentForwarderAgentMethod ShipmentForwarderAgentMethod { get; set; }

        public User User { get; set; }

    }
}