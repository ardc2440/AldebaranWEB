using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("STRANSITO", Schema = "dbo")]
    public partial class Stransito
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMTRANSITO { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHAESTRECIBO { get; set; }

        [ConcurrencyCheck]
        public int? CANTIDADREC { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHA { get; set; }

        [ConcurrencyCheck]
        public string ACTIVIDAD { get; set; }

        [ConcurrencyCheck]
        public int? IDITEMXCOLOR { get; set; }

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