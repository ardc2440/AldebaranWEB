using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ANULAPROCESO", Schema = "dbo")]
    public partial class HisAnulaproceso
    {
        [Required]
        [ConcurrencyCheck]
        public int IDPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Key]
        [Required]
        public int IDANULAPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAANULAPROCESO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public int? IDSATELITE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL584 { get; set; }

    }
}