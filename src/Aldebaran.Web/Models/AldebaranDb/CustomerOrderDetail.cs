using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_order_details", Schema = "dbo")]
    public class CustomerOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ORDER_DETAIL_ID { get; set; }

        [Required]
        public int CUSTOMER_ORDER_ID { get; set; }

        [Required]
        public int REFERENCE_ID { get; set; }

        [Required]
        public int REQUESTED_QUANTITY { get; set; }

        [Required]
        public int PROCESSED_QUANTITY { get; set; }

        [Required]
        public int DELIVERED_QUANTITY { get; set; }

        public string BRAND { get; set; }

        public CustomerOrder CustomerOrder { get; set; }

        public ItemReference ItemReference { get; set; }
    }
}
