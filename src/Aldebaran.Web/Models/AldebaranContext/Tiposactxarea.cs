using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("TIPOSACTXAREA", Schema = "dbo")]
    public partial class Tiposactxarea
    {
        [Key]
        [Required]
        public int IDTIPOACTIVIDAD { get; set; }

        [Key]
        [Required]
        public int IDAREA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL643 { get; set; }

        public Area Area { get; set; }

        public Tiposactividad Tiposactividad { get; set; }

    }
}