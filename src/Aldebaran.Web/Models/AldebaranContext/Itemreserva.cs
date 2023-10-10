using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMRESERVAS", Schema = "dbo")]
    public partial class Itemreserva
    {
        [Key]
        [Required]
        public int IDDETRESERVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDRESERVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADRESERV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public float PRECIOVENTA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [ConcurrencyCheck]
        public string MARCA { get; set; }

        [ConcurrencyCheck]
        public string ENVIARAPED { get; set; }

        [ConcurrencyCheck]
        public string NUMPEDIDO { get; set; }

        [ConcurrencyCheck]
        public string ESTADO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL600 { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Linea Linea { get; set; }

        public Reserva Reserva { get; set; }

    }
}