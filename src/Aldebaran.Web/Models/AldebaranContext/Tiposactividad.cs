using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("TIPOSACTIVIDAD", Schema = "dbo")]
    public partial class Tiposactividad
    {
        [Key]
        [Required]
        public int IDTIPOACTIVIDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACTIVIDAD { get; set; }

        [ConcurrencyCheck]
        public string TRIAL643 { get; set; }

        public ICollection<Actxactpedido> Actxactpedidos { get; set; }

        public ICollection<Tiposactxarea> Tiposactxareas { get; set; }

    }
}