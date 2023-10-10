using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("SUBITEMDETENVIO", Schema = "dbo")]
    public partial class Subitemdetenvio
    {
        [Key]
        [Required]
        public int IDSUBITEMDETENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDETENVIO { get; set; }

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
        public int CANTIDADENVIADA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL643 { get; set; }

        public Bodega Bodega { get; set; }

        public Envio Envio { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Itemsxcolor Itemsxcolor1 { get; set; }

    }
}