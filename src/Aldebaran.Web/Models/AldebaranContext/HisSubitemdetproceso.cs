using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_SUBITEMDETPROCESO", Schema = "dbo")]
    public partial class HisSubitemdetproceso
    {
        [Key]
        [Required]
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
        public int CANTIDADPROCESO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL590 { get; set; }

    }
}