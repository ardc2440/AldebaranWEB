using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CLIENTES", Schema = "dbo")]
    public partial class Cliente
    {
        [Key]
        [Required]
        public int IDCLIENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTIPIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMCLIENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string TELEFONO1 { get; set; }

        [ConcurrencyCheck]
        public string TELEFONO2 { get; set; }

        [ConcurrencyCheck]
        public string FAX { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string DIRECCION { get; set; }

        [ConcurrencyCheck]
        public string CELULAR { get; set; }

        [ConcurrencyCheck]
        public string MAIL1 { get; set; }

        [ConcurrencyCheck]
        public string MAIL2 { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDCIUDAD { get; set; }

        [ConcurrencyCheck]
        public string MAIL3 { get; set; }

        [ConcurrencyCheck]
        public string ENVIARMAIL { get; set; }

        [ConcurrencyCheck]
        public string TRIAL535 { get; set; }

        public Ciudade Ciudade { get; set; }

        public Tipidentifica Tipidentifica { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }

        public ICollection<Reserva> Reservas { get; set; }

    }
}