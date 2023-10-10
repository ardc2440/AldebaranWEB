using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("VALIDACIONCOMPROMETIDOS", Schema = "dbo")]
    public partial class Validacioncomprometido
    {
        [Key]
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public int? PEDIDOINV { get; set; }

        [ConcurrencyCheck]
        public int? COMPROMETIDO { get; set; }

        [ConcurrencyCheck]
        public int? CANTPROCESO { get; set; }

        [ConcurrencyCheck]
        public int? PROCESO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL646 { get; set; }

    }
}