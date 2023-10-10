using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ENTREGASPACT", Schema = "dbo")]
    public partial class Entregaspact
    {
        [Key]
        [Required]
        public int IDENTREGAPACT { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHADESCARGUE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAPACTADA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL548 { get; set; }

        public ICollection<Detentregaspact> Detentregaspacts { get; set; }

        public Funcionario Funcionario { get; set; }

        public Pedido Pedido { get; set; }

    }
}