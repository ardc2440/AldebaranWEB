using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_order_activities", Schema = "dbo")]
    public partial class CustomerOrderActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ORDER_ACTIVITY_ID { get; set; }

        [Required]
        public int CUSTOMER_ORDER_ID { get; set; }

        [Required]
        public short AREA_ID { get; set; }

        [Required]
        public int EMPLOYEE_ID { get; set; }

        [Required]
        public DateTime ACTIVITY_DATE { get; set; }

        public string NOTES { get; set; }

        public CustomerOrder CustomerOrder { get; set; }

        public Area Area { get; set; }

        public Employee Employee { get; set; }

        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }
    }
}
