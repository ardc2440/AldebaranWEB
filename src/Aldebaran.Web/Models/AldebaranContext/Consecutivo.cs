using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CONSECUTIVO", Schema = "dbo")]
    public partial class Consecutivo
    {
        [Key]
        [Required]
        public string NOMTABLA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CONSTABLA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL535 { get; set; }

    }
}