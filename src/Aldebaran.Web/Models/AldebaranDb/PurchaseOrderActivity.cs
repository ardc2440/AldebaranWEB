using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("purchase_order_activities", Schema = "dbo")]
    public partial class PurchaseOrderActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PURCHASE_ORDER_ACTIVITY_ID { get; set; }

        [Required]
        public int PURCHASE_ORDER_ID { get; set; }

        public DateTime EXECUTION_DATE { get; set; }

        [Required]
        public string ACTIVITY_DESCRIPTION { get; set; }

        public DateTime CREATION_DATE { get; set; }

        [Required]
        public int EMPLOYEE_ID { get; set; }

        [Required]
        public int ACTIVITY_EMPLOYEE_ID { get; set; }

        public Employee ActivityEmployee { get; set; }

        public Employee Employee { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

    }
}