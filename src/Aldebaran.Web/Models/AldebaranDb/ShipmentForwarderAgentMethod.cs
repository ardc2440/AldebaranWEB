using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("shipment_forwarder_agent_methods", Schema = "dbo")]
    public partial class ShipmentForwarderAgentMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short SHIPMENT_FORWARDER_AGENT_METHOD_ID { get; set; }

        [Required]
        public short SHIPMENT_METHOD_ID { get; set; }

        [Required]
        public int FORWARDER_AGENT_ID { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        public ForwarderAgent ForwarderAgent { get; set; }

        public ShipmentMethod ShipmentMethod { get; set; }

    }
}