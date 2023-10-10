using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ANULAPROCESO", Schema = "dbo")]
    public partial class Anulaproceso
    {
        [Required]
        [ConcurrencyCheck]
        public int IDPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Key]
        [Required]
        public int IDANULAPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAANULAPROCESO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public int? IDSATELITE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL525 { get; set; }

        public Pedido Pedido { get; set; }

        public Cantproceso Cantproceso { get; set; }

        public Satelite Satelite { get; set; }

        public ICollection<Anulasubitemdetproceso> Anulasubitemdetprocesos { get; set; }

    }
}