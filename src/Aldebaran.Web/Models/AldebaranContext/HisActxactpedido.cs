using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ACTXACTPEDIDOS", Schema = "dbo")]
    public partial class HisActxactpedido
    {
        [Key]
        [Required]
        public int IDTIPOACTIVIDAD { get; set; }

        [Key]
        [Required]
        public int IDACTPEDIDO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL584 { get; set; }

    }
}