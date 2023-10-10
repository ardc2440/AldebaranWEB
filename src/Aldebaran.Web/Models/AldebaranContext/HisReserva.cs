using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("HIS_RESERVAS", Schema = "dbo")]
    public partial class HisReserva
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
        public string TRIAL590 { get; set; }

    }
}