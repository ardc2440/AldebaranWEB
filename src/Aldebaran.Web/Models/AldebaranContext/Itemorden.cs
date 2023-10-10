using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMORDEN", Schema = "dbo")]
    public partial class Itemorden
    {
        [Key]
        [Required]
        public int IDITEMORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADSOLIC { get; set; }

        [ConcurrencyCheck]
        public int? IDBODEGA { get; set; }

        [ConcurrencyCheck]
        public int? CANTIDADREC { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [ConcurrencyCheck]
        public string TRIAL594 { get; set; }

        public Bodega Bodega { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Ordene Ordene { get; set; }

    }
}