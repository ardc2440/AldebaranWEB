using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MODPEDIDOS", Schema = "dbo")]
    public partial class Modpedido
    {
        [Key]
        [Required]
        public int IDPEDIDO { get; set; }

        [Key]
        [Required]
        public int IDFUNCIONARIO { get; set; }

        [Key]
        public DateTime FECHA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public Funcionario Funcionario { get; set; }

        public Pedido Pedido { get; set; }

    }
}