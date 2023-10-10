using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ANULACIONRESERVA", Schema = "dbo")]
    public partial class Anulacionreserva
    {
        [Key]
        [Required]
        public int IDRESERVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMOTIVO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL522 { get; set; }

        public Funcionario Funcionario { get; set; }

        public Motivodevolucion Motivodevolucion { get; set; }

        public Reserva Reserva { get; set; }

    }
}