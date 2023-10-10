using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_ITEMRESERVAS", Schema = "dbo")]
    public partial class HisItemreserva
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
        public string TRIAL590 { get; set; }

    }
}