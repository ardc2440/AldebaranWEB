using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("RUNIDADESMEDIDA", Schema = "dbo")]
    public partial class Runidadesmedidum
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDUNIDAD { get; set; }

        [ConcurrencyCheck]
        public string NOMUNIDAD { get; set; }

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