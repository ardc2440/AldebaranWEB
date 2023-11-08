using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_order_activity_details", Schema = "dbo")]
    public partial class CustomerOrderActivityDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ORDER_ACTIVITY_DETAIL_ID { get; set; }

        [Required]
        public int CUSTOMER_ORDER_ACTIVITY_ID { get; set; }

        [Required]
        public short ACTIVITY_TYPE_ID { get; set; }

        [Required]
        public int EMPLOYEE_ID { get; set; }

        [Required]
        public int ACTIVITY_EMPLOYEE_ID { get; set; }

        public CustomerOrderActivity CustomerOrderActivity { get; set; }

        public ActivityType ActivityType { get; set; }

        public Employee Employee { get; set; }

        public Employee EmployeeActivity { get; set; }
    }
}
