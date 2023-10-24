using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("purchase_order_details", Schema = "dbo")]
    public partial class PurchaseOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PURCHASE_ORDER_DETAIL_ID { get; set; }

        [Required]
        public int PURCHASE_ORDER_ID { get; set; }

        [Required]
        public int REFERENCE_ID { get; set; }

        [Required]
        public short WAREHOUSE_ID { get; set; }

        [Required]
        public int? RECEIVED_QUANTITY { get; set; }

        [Required]
        public int REQUESTED_QUANTITY { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public ItemReference ItemReference { get; set; }

        public Warehouse Warehouse { get; set; }

    }
}