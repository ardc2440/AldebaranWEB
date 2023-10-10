using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("OPCIONES", Schema = "dbo")]
    public partial class Opcione
    {
        [Key]
        [Required]
        public int IDOPCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBREOPCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBREFORMA { get; set; }

        [ConcurrencyCheck]
        public string NOMBREMENU { get; set; }

        [ConcurrencyCheck]
        public string NOMBRETOOL { get; set; }

        [ConcurrencyCheck]
        public int? IDPADRE { get; set; }

        [ConcurrencyCheck]
        public string CONSULTA { get; set; }

        [ConcurrencyCheck]
        public string ADICIONA { get; set; }

        [ConcurrencyCheck]
        public string MODIFICA { get; set; }

        [ConcurrencyCheck]
        public string ELIMINA { get; set; }

        [ConcurrencyCheck]
        public string IMPRIME { get; set; }

        [ConcurrencyCheck]
        public string TRIAL623 { get; set; }

        public ICollection<Grupopc> Grupopcs { get; set; }

    }
}