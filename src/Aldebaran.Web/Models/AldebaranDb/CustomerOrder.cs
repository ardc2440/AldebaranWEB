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

        public ICollection<CustomerReservation> CustomerReservations { get; set; }
    }
}
