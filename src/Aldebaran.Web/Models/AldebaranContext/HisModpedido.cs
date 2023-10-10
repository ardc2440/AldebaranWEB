using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_MODPEDIDOS", Schema = "dbo")]
    public partial class HisModpedido
    {
        [Key]
        [Required]
        public int IDPEDIDO { get; set; }

        [Key]
        [Required]
        public int IDFUNCIONARIO { get; set; }

        [Key]
        public DateTime FECHA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL590 { get; set; }

    }
}