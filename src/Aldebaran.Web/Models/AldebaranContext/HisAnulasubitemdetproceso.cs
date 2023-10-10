using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ANULASUBITEMDETPROCESO", Schema = "dbo")]
    public partial class HisAnulasubitemdetproceso
    {
        [Required]
        [ConcurrencyCheck]
        public int IDANULAPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDETANULAPROCESO { get; set; }

        [Key]
        [Required]
        public int IDANSUBITEMDETPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDSUBITEMDETPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDETPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMARMADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADANULADA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL584 { get; set; }

    }
}