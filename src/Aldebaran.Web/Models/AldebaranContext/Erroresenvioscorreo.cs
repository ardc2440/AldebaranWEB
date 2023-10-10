using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ERRORESENVIOSCORREO", Schema = "dbo")]
    public partial class Erroresenvioscorreo
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
        public string TRIAL577 { get; set; }

    }
}