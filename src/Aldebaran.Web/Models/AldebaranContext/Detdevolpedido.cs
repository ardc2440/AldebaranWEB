using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("DETDEVOLPEDIDO", Schema = "dbo")]
    public partial class Detdevolpedido
    {
        [Key]
        [Required]
        public int IDDETDEVOLPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDEVOLPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADDEV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVODEV { get; set; }

        [ConcurrencyCheck]
        public string TRIAL541 { get; set; }

        public Devolpedido Devolpedido { get; set; }

        public Motivodevolucion Motivodevolucion { get; set; }

        public Itemsxbodega Itemsxbodega { get; set; }

    }
}