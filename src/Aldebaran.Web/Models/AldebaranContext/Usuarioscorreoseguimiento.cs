using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("USUARIOSCORREOSEGUIMIENTO", Schema = "dbo")]
    public partial class Usuarioscorreoseguimiento
    {
        [Key]
        [Required]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CORREOSALIDA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL646 { get; set; }

    }
}