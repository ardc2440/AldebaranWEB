using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("DEPARTAMENTOS", Schema = "dbo")]
    public partial class Departamento
    {
        [Key]
        [Required]
        public int IDDEPTO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMDEPTO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL538 { get; set; }

        public ICollection<Ciudade> Ciudades { get; set; }

    }
}