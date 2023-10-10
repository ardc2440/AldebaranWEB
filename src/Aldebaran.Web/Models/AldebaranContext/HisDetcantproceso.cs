using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_DETCANTPROCESO", Schema = "dbo")]
    public partial class HisDetcantproceso
    {
        [Key]
        [Required]
        public int IDDETCANTPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public int? IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADPROCESO { get; set; }

        [ConcurrencyCheck]
        public int? IDLINEA { get; set; }

        [ConcurrencyCheck]
        public int? IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [ConcurrencyCheck]
        public string MARCA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL587 { get; set; }

    }
}