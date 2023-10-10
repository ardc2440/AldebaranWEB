using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MODORDENES", Schema = "dbo")]
    public partial class Modordene
    {
        [Required]
        [ConcurrencyCheck]
        public int IDORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Key]
        [Required]
        public int IDMODORDEN { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public Funcionario Funcionario { get; set; }

        public Ordene Ordene { get; set; }

    }
}