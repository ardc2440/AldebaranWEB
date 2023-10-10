using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("RESERVAS", Schema = "dbo")]
    public partial class Reserva
    {
        [Key]
        [Required]
        public int IDRESERVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDCLIENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHARESERVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHACADUCIDAD { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public int? IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ESTADO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string NUMRESERVA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL636 { get; set; }

        public ICollection<Anulacionreserva> Anulacionreservas { get; set; }

        public ICollection<Itemreserva> Itemreservas { get; set; }

        public ICollection<Modreserva> Modreservas { get; set; }

        public Cliente Cliente { get; set; }

        public Funcionario Funcionario { get; set; }

        public Pedido Pedido { get; set; }

    }
}