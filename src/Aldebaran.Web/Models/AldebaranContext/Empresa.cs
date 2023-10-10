using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("EMPRESAS", Schema = "dbo")]
    public partial class Empresa
    {
        [Key]
        [Required]
        public int IDEMPRESA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMEMPRESA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL548 { get; set; }

        public ICollection<Ordene> Ordenes { get; set; }

    }
}