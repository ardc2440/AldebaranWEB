using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ALARMAS", Schema = "dbo")]
    public partial class Alarma
    {
        [Key]
        [Required]
        public int IDALARMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTIPOALARMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDOC { get; set; }

        [ConcurrencyCheck]
        public string ACTIVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMENSAJEALARMA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL522 { get; set; }

        public Tiposalarma Tiposalarma { get; set; }

    }
}