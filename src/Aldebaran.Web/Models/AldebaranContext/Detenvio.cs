using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("DETENVIO", Schema = "dbo")]
    public partial class Detenvio
    {
        [Key]
        [Required]
        public int IDDETENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public int? IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADENVIADA { get; set; }

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
        public string TRIAL541 { get; set; }

        public Bodega Bodega { get; set; }

        public Envio Envio { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Linea Linea { get; set; }

    }
}