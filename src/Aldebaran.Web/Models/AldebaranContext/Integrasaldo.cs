using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("INTEGRASALDOS", Schema = "dbo")]
    public partial class Integrasaldo
    {
        [Key]
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Key]
        [Required]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string COLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string BODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACCION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL590 { get; set; }

        public Bodega Bodega { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

    }
}