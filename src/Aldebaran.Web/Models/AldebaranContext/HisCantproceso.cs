using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_CANTPROCESO", Schema = "dbo")]
    public partial class HisCantproceso
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
        public string TRIAL587 { get; set; }

    }
}