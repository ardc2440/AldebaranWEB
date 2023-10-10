using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("DETENTREGASPACT", Schema = "dbo")]
    public partial class Detentregaspact
    {
        [Key]
        [Required]
        public int IDDETENTREGAPACT { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDENTREGAPACT { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADPACTADA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL541 { get; set; }

        public Entregaspact Entregaspact { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

    }
}