using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMPEDIDOAGOTADO", Schema = "dbo")]
    public partial class Itempedidoagotado
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

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [ConcurrencyCheck]
        public string MARCA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL600 { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Linea Linea { get; set; }

        public Pedido Pedido { get; set; }

    }
}