using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("REMBALAJES", Schema = "dbo")]
    public partial class Rembalaje
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDEMBALAJE { get; set; }

        [ConcurrencyCheck]
        public int? IDITEM { get; set; }

        [ConcurrencyCheck]
        public float? PESO { get; set; }

        [ConcurrencyCheck]
        public float? ALTURA { get; set; }

        [ConcurrencyCheck]
        public float? ANCHO { get; set; }

        [ConcurrencyCheck]
        public float? LARGO { get; set; }

        [ConcurrencyCheck]
        public string CANTIDAD { get; set; }

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
        public string TRIAL636 { get; set; }

    }
}