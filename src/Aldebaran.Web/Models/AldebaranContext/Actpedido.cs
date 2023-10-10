using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ACTPEDIDO", Schema = "dbo")]
    public partial class Actpedido
    {
        [Key]
        [Required]
        public int IDACTPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDAREA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string TRIAL509 { get; set; }

        public Area Area { get; set; }

        public Funcionario Funcionario { get; set; }

        public Pedido Pedido { get; set; }

        public ICollection<Actxactpedido> Actxactpedidos { get; set; }

    }
}