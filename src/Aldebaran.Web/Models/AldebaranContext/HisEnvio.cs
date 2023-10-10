using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ENVIOS", Schema = "dbo")]
    public partial class HisEnvio
    {
        [Key]
        [Required]
        public int IDENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMETODOENV { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string NUMGUIA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REMISION { get; set; }

        [ConcurrencyCheck]
        public string LOGIN { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHAHORA { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL587 { get; set; }

    }
}