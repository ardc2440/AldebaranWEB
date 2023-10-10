using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CANTPROCESO", Schema = "dbo")]
    public partial class Cantproceso
    {
        [Key]
        [Required]
        public int IDPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAPROCESO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public int? IDSATELITE { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHAHORATRASLADO { get; set; }

        [ConcurrencyCheck]
        public string NOMRECIBE { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL528 { get; set; }

        public ICollection<Anuladetcantproceso> Anuladetcantprocesos { get; set; }

        public ICollection<Anulaproceso> Anulaprocesos { get; set; }

        public ICollection<Anulasubitemdetproceso> Anulasubitemdetprocesos { get; set; }

        public Pedido Pedido { get; set; }

        public Satelite Satelite { get; set; }

        public ICollection<Detcantproceso> Detcantprocesos { get; set; }

        public ICollection<Subitemdetproceso> Subitemdetprocesos { get; set; }

    }
}