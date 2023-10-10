using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("DEVOLORDEN", Schema = "dbo")]
    public partial class Devolorden
    {
        [Key]
        [Required]
        public int IDDEVOLORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVODEV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHADEV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL545 { get; set; }

        public Motivodevolucion Motivodevolucion { get; set; }

        public Ordene Ordene { get; set; }

    }
}