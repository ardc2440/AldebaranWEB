using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MENSAJESALARMAS", Schema = "dbo")]
    public partial class Mensajesalarma
    {
        [Key]
        [Required]
        public int IDMENSAJE { get; set; }

        [Key]
        [Required]
        public int IDTIPOALARMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string MENSAJE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

    }
}