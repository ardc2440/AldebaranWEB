using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MOTIVODEVOLUCION", Schema = "dbo")]
    public partial class Motivodevolucion
    {
        [Key]
        [Required]
        public int IDMOTIVODEV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMMOTIVODEV { get; set; }

        [ConcurrencyCheck]
        public string DESCMOTIVODEV { get; set; }

        [ConcurrencyCheck]
        public string TRIAL623 { get; set; }

        public ICollection<Anulacionreserva> Anulacionreservas { get; set; }

        public ICollection<Cancelpedido> Cancelpedidos { get; set; }

        public ICollection<Detdevolpedido> Detdevolpedidos { get; set; }

        public ICollection<Devolorden> Devolordens { get; set; }

        public ICollection<Devolpedido> Devolpedidos { get; set; }

    }
}