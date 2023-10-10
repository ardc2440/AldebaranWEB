using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CONTROLCONCURRENCIA", Schema = "dbo")]
    public partial class Controlconcurrencium
    {
        [Key]
        [Required]
        public string TIPO { get; set; }

        [Key]
        [Required]
        public int IDTABLA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBREUSUARIO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHABLOQUEO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL538 { get; set; }

    }
}