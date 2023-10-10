using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("TIPIDENTIFICA", Schema = "dbo")]
    public partial class Tipidentifica
    {
        [Key]
        [Required]
        public int IDTIPIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CODTIPIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMTIPIDENTIFICA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL643 { get; set; }

        public ICollection<Cliente> Clientes { get; set; }

        public ICollection<Funcionario> Funcionarios { get; set; }

        public ICollection<Proveedore> Proveedores { get; set; }

        public ICollection<Satelite> Satelites { get; set; }

    }
}