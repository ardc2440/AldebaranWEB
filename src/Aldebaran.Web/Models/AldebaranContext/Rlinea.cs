using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("RLINEAS", Schema = "dbo")]
    public partial class Rlinea
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [ConcurrencyCheck]
        public string CODLINEA { get; set; }

        [ConcurrencyCheck]
        public string NOMLINEA { get; set; }

        [ConcurrencyCheck]
        public string DEMONIO { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int INTENTOS { get; set; }

        [ConcurrencyCheck]
        public string ERROR { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHA_INTEGRA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL639 { get; set; }

    }
}