using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AJUSTES", Schema = "dbo")]
    public partial class Ajuste
    {
        [Key]
        [Required]
        public int IDAJUSTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVAJUSTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string TIPOAJUSTE { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL512 { get; set; }

        public Funcionario Funcionario { get; set; }

        public Motivajuste Motivajuste { get; set; }

        public ICollection<Ajustesxitem> Ajustesxitems { get; set; }

    }
}