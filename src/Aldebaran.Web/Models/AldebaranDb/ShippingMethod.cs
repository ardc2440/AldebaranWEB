using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("shipping_methods", Schema = "dbo")]
    public partial class ShippingMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short SHIPPING_METHOD_ID { get; set; }

        [Required]
        public string SHIPPING_METHOD_NAME { get; set; }

        public string SHIPPING_METHOD_NOTES { get; set; }

    }
}