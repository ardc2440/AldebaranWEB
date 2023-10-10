using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ERPDOCUMENTTYPE", Schema = "dbo")]
    public partial class Erpdocumenttype
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NAME { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CODETYPE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime LASTEXECUTIONDATE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime LASTCLEANINGDATE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL577 { get; set; }

        public ICollection<Erpshippingprocessdetail> Erpshippingprocessdetails { get; set; }

    }
}