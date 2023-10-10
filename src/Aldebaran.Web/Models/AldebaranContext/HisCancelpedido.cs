using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_CANCELPEDIDO", Schema = "dbo")]
    public partial class HisCancelpedido
    {
        [Key]
        [Required]
        public int IDCANCELPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHACANC { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL587 { get; set; }

    }
}