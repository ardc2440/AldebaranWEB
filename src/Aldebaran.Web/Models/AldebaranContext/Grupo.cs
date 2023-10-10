using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("GRUPOS", Schema = "dbo")]
    public partial class Grupo
    {
        [Key]
        [Required]
        public int IDGRUPO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMGRUPO { get; set; }

        [ConcurrencyCheck]
        public string DESCRIPCION { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL581 { get; set; }

        public ICollection<Grupopc> Grupopcs { get; set; }

        public ICollection<Grupusu> Grupusus { get; set; }

        public ICollection<Horario> Horarios { get; set; }

    }
}