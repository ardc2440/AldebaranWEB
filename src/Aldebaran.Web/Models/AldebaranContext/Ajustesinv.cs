using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AJUSTESINV", Schema = "dbo")]
    public partial class Ajustesinv
    {
        [Required]
        public int ANNO { get; set; }

        [Required]
        public int SEMESTRE { get; set; }

        [Required]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        public int IDBODEGA { get; set; }

        public int? CANTINI { get; set; }

        [Required]
        public int CANTIDAD { get; set; }

        public DateTime? FECHA { get; set; }

        public string TRIAL518 { get; set; }

    }
}