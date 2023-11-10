using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_order_in_process_Details", Schema = "dbo")]
    public class CustomerOrderInProcessDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ORDER_IN_PROCESS_DETAIL_ID { get; set; }

        [Required]
        public int CUSTOMER_ORDER_IN_PROCESS_ID { get; set; }

        [Required]
        public int CUSTOMER_ORDER_DETAIL_ID { get; set; }

        [Required]
        public short WAREHOUSE_ID { get; set; }

        [Required]
        public int PROCESSED_QUANTITY { get; set; }

        public string BRAND { get; set; }

        public CustomerOrderInProcess CustomerOrderInProcess { get; set; }

        public CustomerOrderDetail CustomerOrderDetail { get; set; }

        public Warehouse Warehouse { get; set; }
    }
}
