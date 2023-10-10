using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("USUARIOS", Schema = "dbo")]
    public partial class Usuario
    {
        [Key]
        [Required]
        public int IDUSUARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string LOGIN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string USAHORARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string APLICANFESTIVOS { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CLAVE1 { get; set; }

        [ConcurrencyCheck]
        public string TRIAL646 { get; set; }

        public ICollection<Grupusu> Grupusus { get; set; }

        public Funcionario Funcionario { get; set; }

    }
}