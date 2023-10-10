using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ANULASUBITEMDETPROCESO", Schema = "dbo")]
    public partial class Anulasubitemdetproceso
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
        public string TRIAL525 { get; set; }

        public Anulaproceso Anulaproceso { get; set; }

        public Bodega Bodega { get; set; }

        public Anuladetcantproceso Anuladetcantproceso { get; set; }

        public Detcantproceso Detcantproceso { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Itemsxcolor Itemsxcolor1 { get; set; }

        public Cantproceso Cantproceso { get; set; }

    }
}