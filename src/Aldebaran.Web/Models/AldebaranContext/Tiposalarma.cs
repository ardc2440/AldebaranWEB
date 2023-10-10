using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("TIPOSALARMAS", Schema = "dbo")]
    public partial class Tiposalarma
    {
        [Key]
        [Required]
        public int IDTIPO { get; set; }

        [ConcurrencyCheck]
        public string DESCRIPCION { get; set; }

        [ConcurrencyCheck]
        public int? ALARMAPORDEFECTO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string TABLA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CAMPO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL643 { get; set; }

        public ICollection<Alarma> Alarmas { get; set; }

        public ICollection<Permisosalarma> Permisosalarmas { get; set; }

    }
}