using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_PEDIDOS", Schema = "dbo")]
    public partial class HisPedido
    {
        [Key]
        [Required]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDCLIENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAESTENTREGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ENTREGAPACTADA { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ESTADO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONESCLIENTE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL590 { get; set; }

    }
}