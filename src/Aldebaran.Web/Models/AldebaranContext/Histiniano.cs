using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HISTINIANO", Schema = "dbo")]
    public partial class Histiniano
    {
        [Key]
        [Required]
        public int ANNO { get; set; }

        [Key]
        [Required]
        public int SEMESTRE { get; set; }

        [Key]
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [Key]
        [Required]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [ConcurrencyCheck]
        public string TRIAL584 { get; set; }

    }
}