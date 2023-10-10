using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("SUBITEMDETPROCESO", Schema = "dbo")]
    public partial class Subitemdetproceso
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
        public string TRIAL643 { get; set; }

        public Bodega Bodega { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Itemsxcolor Itemsxcolor1 { get; set; }

        public Cantproceso Cantproceso { get; set; }

    }
}