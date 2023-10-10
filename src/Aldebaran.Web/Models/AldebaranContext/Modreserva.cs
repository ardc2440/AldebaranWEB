using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MODRESERVAS", Schema = "dbo")]
    public partial class Modreserva
    {
        [Key]
        [Required]
        public int IDRESERVA { get; set; }

        [Key]
        [Required]
        public int IDFUNCIONARIO { get; set; }

        [Key]
        public DateTime FECHA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL623 { get; set; }

        public Funcionario Funcionario { get; set; }

        public Reserva Reserva { get; set; }

    }
}