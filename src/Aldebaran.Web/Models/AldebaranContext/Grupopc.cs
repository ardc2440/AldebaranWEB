using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("GRUPOPC", Schema = "dbo")]
    public partial class Grupopc
    {
        [Key]
        [Required]
        public int IDGRUPO { get; set; }

        [Key]
        [Required]
        public int IDOPCION { get; set; }

        [ConcurrencyCheck]
        public string ADICIONA { get; set; }

        [ConcurrencyCheck]
        public string MODIFICA { get; set; }

        [ConcurrencyCheck]
        public string ELIMINA { get; set; }

        [ConcurrencyCheck]
        public string CONSULTA { get; set; }

        [ConcurrencyCheck]
        public string IMPRIME { get; set; }

        [ConcurrencyCheck]
        public string TRIAL581 { get; set; }

        public Grupo Grupo { get; set; }

        public Opcione Opcione { get; set; }

    }
}