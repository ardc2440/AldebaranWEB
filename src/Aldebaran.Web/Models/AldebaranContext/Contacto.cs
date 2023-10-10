using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CONTACTOS", Schema = "dbo")]
    public partial class Contacto
    {
        [Key]
        [Required]
        public int IDCONTACTO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDCLIENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMCONTACTO { get; set; }

        [ConcurrencyCheck]
        public string CARGO { get; set; }

        [ConcurrencyCheck]
        public string TELEFONO { get; set; }

        [ConcurrencyCheck]
        public string MAIL { get; set; }

        [ConcurrencyCheck]
        public string TRIAL538 { get; set; }

    }
}