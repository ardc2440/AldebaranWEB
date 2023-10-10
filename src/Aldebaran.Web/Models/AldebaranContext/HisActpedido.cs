using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ACTPEDIDO", Schema = "dbo")]
    public partial class HisActpedido
    {
        [Key]
        [Required]
        public int IDACTPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDAREA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string TRIAL584 { get; set; }

    }
}