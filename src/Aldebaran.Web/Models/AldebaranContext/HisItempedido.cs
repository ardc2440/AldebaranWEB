using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ITEMPEDIDO", Schema = "dbo")]
    public partial class HisItempedido
    {
        [Key]
        [Required]
        public int IDITEMPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [ConcurrencyCheck]
        public float? PRECIOVENTA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADENT { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [ConcurrencyCheck]
        public string MARCA { get; set; }

        [ConcurrencyCheck]
        public string CANTIDADENPROC { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTPROCESO { get; set; }

        [ConcurrencyCheck]
        public string ESTADO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL587 { get; set; }

    }
}