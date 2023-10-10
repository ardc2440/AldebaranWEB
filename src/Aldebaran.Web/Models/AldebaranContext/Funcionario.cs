using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("FUNCIONARIOS", Schema = "dbo")]
    public partial class Funcionario
    {
        [Key]
        [Required]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTIPIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBRECOMPLETO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CARGO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDAREA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL581 { get; set; }

        public ICollection<Actpedido> Actpedidos { get; set; }

        public ICollection<Ajuste> Ajustes { get; set; }

        public ICollection<Anulacionreserva> Anulacionreservas { get; set; }

        public ICollection<Cancelpedido> Cancelpedidos { get; set; }

        public ICollection<Devolpedido> Devolpedidos { get; set; }

        public ICollection<Entregaspact> Entregaspacts { get; set; }

        public ICollection<Envioscorreo> Envioscorreos { get; set; }

        public Area Area { get; set; }

        public Tipidentifica Tipidentifica { get; set; }

        public ICollection<Logalarmascantidadesminima> Logalarmascantidadesminimas { get; set; }

        public ICollection<Modordene> Modordenes { get; set; }

        public ICollection<Modpedido> Modpedidos { get; set; }

        public ICollection<Modreserva> Modreservas { get; set; }

        public ICollection<Ordene> Ordenes { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }

        public ICollection<Reserva> Reservas { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }

    }
}