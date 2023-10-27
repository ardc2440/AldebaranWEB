using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_reservations", Schema = "dbo")]

    public partial class CustomerReservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_RESERVATION_ID { get; set; }

        [Required]
        public int CUSTOMER_ID { get; set; }

        [Required]
        public string RESERVATION_NUMBER { get; set; }

        [Required]
        public DateTime RESERVATION_DATE { get; set; }

        [Required]
        public DateTime EXPIRATION_DATE { get; set; }
        public string NOTES { get; set; }

        [Required]
        public int EMPLOYEE_ID { get; set; }

        public int CUSTOMER_ORDER_ID { get; set; }

        [Required]
        public short STATUS_DOCUMENT_TYPE_ID { get; set; }

        [Required]
        public DateTime CREATION_DATE { get; set; }

        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }

        public Customer Customer { get; set; }

        public StatusDocumentType StatusDocumentType { get; set; }

        public Employee Employee { get; set; }
    }
}
