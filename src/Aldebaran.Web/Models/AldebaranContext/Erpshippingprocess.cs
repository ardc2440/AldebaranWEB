using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ERPSHIPPINGPROCESS", Schema = "dbo")]
    public partial class Erpshippingprocess
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string DESTINATIONPATH { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime EXECUTIONDATE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL577 { get; set; }

        public ICollection<Erpshippingprocessdetail> Erpshippingprocessdetails { get; set; }

    }
}