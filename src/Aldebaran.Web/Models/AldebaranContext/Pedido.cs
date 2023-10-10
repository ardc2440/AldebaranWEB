using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("PEDIDOS", Schema = "dbo")]
    public partial class Pedido
    {
        [Key]
        [Required]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDCLIENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAESTENTREGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ENTREGAPACTADA { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ESTADO { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONESCLIENTE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL626 { get; set; }

        public ICollection<Actpedido> Actpedidos { get; set; }

        public ICollection<Anulaproceso> Anulaprocesos { get; set; }

        public ICollection<Cancelpedido> Cancelpedidos { get; set; }

        public ICollection<Cantproceso> Cantprocesos { get; set; }

        public ICollection<Cierrepedido> Cierrepedidos { get; set; }

        public ICollection<Devolpedido> Devolpedidos { get; set; }

        public ICollection<Entregaspact> Entregaspacts { get; set; }

        public ICollection<Envio> Envios { get; set; }

        public ICollection<Itempedido> Itempedidos { get; set; }

        public ICollection<Itempedidoagotado> Itempedidoagotados { get; set; }

        public ICollection<Modpedido> Modpedidos { get; set; }

        public Cliente Cliente { get; set; }

        public Funcionario Funcionario { get; set; }

        public ICollection<Reserva> Reservas { get; set; }

    }
}