using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("DEVOLPEDIDO", Schema = "dbo")]
    public partial class Devolpedido
    {
        [Key]
        [Required]
        public int IDDEVOLPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVODEV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHADEV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string TRIAL545 { get; set; }

        public ICollection<Detdevolpedido> Detdevolpedidos { get; set; }

        public Funcionario Funcionario { get; set; }

        public Motivodevolucion Motivodevolucion { get; set; }

        public Pedido Pedido { get; set; }

    }
}