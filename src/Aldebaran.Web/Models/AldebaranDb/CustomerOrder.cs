using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_orders", Schema = "dbo")]
    public partial class CustomerOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ORDER_ID { get; set; }

        [Required]
        public int CUSTOMER_ID { get; set; }

        [Required]
        public string ORDER_NUMBER { get; set; }

        [Required]
        public DateTime ORDER_DATE { get; set; }

        [Required]
        public DateTime ESTIMATED_DELIVERY_DATE { get; set; }

        public string INTERNAL_NOTES { get; set; }

        [Required]
        public int EMPLOYEE_ID { get; set; }

        [Required]
        public short STATUS_DOCUMENT_TYPE_ID { get; set; }

        [Required]
        public DateTime CREATION_DATE { get; set; }

        public string CUSTOMER_NOTES { get; set; }

        public Customer Customer { get; set; }

        public StatusDocumentType StatusDocumentType { get; set; }

        public Employee Employee { get; set; }

        public ICollection<CustomerReservation> CustomerReservations { get; set; }

        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }
    }
}
