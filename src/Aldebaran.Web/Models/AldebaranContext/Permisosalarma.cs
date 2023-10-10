using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("PERMISOSALARMAS", Schema = "dbo")]
    public partial class Permisosalarma
    {
        [Key]
        [Required]
        public int IDTIPOALARMA { get; set; }

        [Key]
        [Required]
        public int IDUSUARIO { get; set; }

        [ConcurrencyCheck]
        public string VISUALIZA { get; set; }

        [ConcurrencyCheck]
        public string DESACTIVA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL636 { get; set; }

        public Tiposalarma Tiposalarma { get; set; }

    }
}