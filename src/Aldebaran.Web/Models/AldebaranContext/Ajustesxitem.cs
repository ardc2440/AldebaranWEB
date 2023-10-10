using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AJUSTESXITEM", Schema = "dbo")]
    public partial class Ajustesxitem
    {
        [Key]
        [Required]
        public int IDDETAJUSTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDAJUSTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL518 { get; set; }

        public Ajuste Ajuste { get; set; }

        public Item Item { get; set; }

        public Linea Linea { get; set; }

    }
}