using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("LOGALARMASCANTIDADESMINIMAS", Schema = "dbo")]
    public partial class Logalarmascantidadesminima
    {
        [Key]
        [Required]
        public int IDALARMA { get; set; }

        [Key]
        [Required]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAREVISION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public Funcionario Funcionario { get; set; }

    }
}