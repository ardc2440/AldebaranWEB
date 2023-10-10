using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ACTORDEN", Schema = "dbo")]
    public partial class Actorden
    {
        [Key]
        [Required]
        public int IDACTIVIDADORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACTIVIDAD { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL505 { get; set; }

        public Ordene Ordene { get; set; }

    }
}