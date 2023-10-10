using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HORARIOS", Schema = "dbo")]
    public partial class Horario
    {
        [Key]
        [Required]
        public int IDHORARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDGRUPO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string DIASEMANA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime INIPRIMPER { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FINPRIMPER { get; set; }

        [ConcurrencyCheck]
        public DateTime? INISEGPER { get; set; }

        [ConcurrencyCheck]
        public DateTime? FINSEGPER { get; set; }

        [ConcurrencyCheck]
        public string TRIAL590 { get; set; }

        public Grupo Grupo { get; set; }

    }
}