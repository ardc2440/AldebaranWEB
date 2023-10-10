using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CANCELPEDIDO", Schema = "dbo")]
    public partial class Cancelpedido
    {
        [Key]
        [Required]
        public int IDCANCELPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHACANC { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL528 { get; set; }

        public Funcionario Funcionario { get; set; }

        public Motivodevolucion Motivodevolucion { get; set; }

        public Pedido Pedido { get; set; }

    }
}