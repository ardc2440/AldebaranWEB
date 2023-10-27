using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    public class CustomerReservationDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_RESERVATION_DETAIL_ID { get; set; }

        [Required]
        public int CUSTOMER_RESERVATION_ID { get; set; }

        [Required]
        public int REFERENCE_ID { get; set; }

        [Required]
        public int RESERVED_QUANTITY { get; set; }

        public string BRAND { get; set; }

        [Required]
        public bool SEND_TO_CUSTOMER_ORDER { get; set; }

        public ItemReference ItemReference { get; set; }

        public CustomerReservation CustomerReservation { get; set; }
    }
}
