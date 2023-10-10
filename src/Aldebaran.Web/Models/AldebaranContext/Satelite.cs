using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("SATELITES", Schema = "dbo")]
    public partial class Satelite
    {
        [Key]
        [Required]
        public int IDSATELITE { get; set; }

        [ConcurrencyCheck]
        public string NOMBRE { get; set; }

        [ConcurrencyCheck]
        public string DIRECCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTIPIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMIDENTIFICA { get; set; }

        [ConcurrencyCheck]
        public string TELEFONO { get; set; }

        [ConcurrencyCheck]
        public string FAX { get; set; }

        [ConcurrencyCheck]
        public string MAIL { get; set; }

        [ConcurrencyCheck]
        public int? IDCIUDAD { get; set; }

        [ConcurrencyCheck]
        public string REPRESENTANTELEGAL { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL639 { get; set; }

        public ICollection<Anulaproceso> Anulaprocesos { get; set; }

        public ICollection<Cantproceso> Cantprocesos { get; set; }

        public Ciudade Ciudade { get; set; }

        public Tipidentifica Tipidentifica { get; set; }

    }
}