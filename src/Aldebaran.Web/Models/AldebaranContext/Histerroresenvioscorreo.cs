using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HISTERRORESENVIOSCORREO", Schema = "dbo")]
    public partial class Histerroresenvioscorreo
    {
        [Key]
        [Required]
        public int IDERRORENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDENVIO { get; set; }

        [ConcurrencyCheck]
        public string ERROR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAERROR { get; set; }

        [ConcurrencyCheck]
        public string TRIAL584 { get; set; }

    }
}