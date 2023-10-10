using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_SUBITEMDETENVIO", Schema = "dbo")]
    public partial class HisSubitemdetenvio
    {
        [Key]
        [Required]
        public int IDSUBITEMDETENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDDETENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMARMADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADENVIADA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL590 { get; set; }

    }
}