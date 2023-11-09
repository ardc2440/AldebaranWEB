using Microsoft.OData.Edm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_orders_in_process", Schema = "dbo")]
    public class CustomerOrderInProcess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ORDER_IN_PROCESS_ID { get; set; }

        [Required]
        public int CUSTOMER_ORDER_ID { get; set; }

        [Required]
        public Date PROCESS_DATE { get; set; }

        [Required]
        public string NOTES { get; set; }

        [Required]
        public int PROCESS_SATELLITE_ID { get; set; }

        [Required]
        public DateTime TRANSFER_DATETIME { get; set; }

        [Required]
        public int EMPLOYEE_RECIPIENT_ID { get; set; }

        [Required]
        public DateTime CREATION_DATE { get; set; }

        public CustomerOrder CustomerOrder { get; set; }

        public Employee Employee { get; set; }

        public ProcessSatellite ProcessSatellite { get; set; }

        public ICollection<CustomerOrderInProcessDetail> CustomerOrderInProcessDetails { get; set; }
    }
}
