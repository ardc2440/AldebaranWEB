using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CIERREPEDIDO", Schema = "dbo")]
    public partial class Cierrepedido
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL535 { get; set; }

        public Pedido Pedido { get; set; }

    }
}