using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ERPSHIPPINGPROCESSDETAIL", Schema = "dbo")]
    public partial class Erpshippingprocessdetail
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDERPSHIPPINGPROCESS { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDERPDOCUMENTTYPE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDOCUMENT { get; set; }

        [ConcurrencyCheck]
        public string FILENAME { get; set; }

        [ConcurrencyCheck]
        public string TRIAL577 { get; set; }

        public Erpdocumenttype Erpdocumenttype { get; set; }

        public Erpshippingprocess Erpshippingprocess { get; set; }

    }
}