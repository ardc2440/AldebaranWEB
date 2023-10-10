using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("RITEMSXCOLOR", Schema = "dbo")]
    public partial class Ritemsxcolor
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public int? IDITEM { get; set; }

        [ConcurrencyCheck]
        public string REFITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public string REFINTITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public string NOMCOLOR { get; set; }

        [ConcurrencyCheck]
        public string NOMITEMXCOLORPROV { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string COLOR { get; set; }

        [ConcurrencyCheck]
        public int? CANTPEDIDA { get; set; }

        [ConcurrencyCheck]
        public int? CANTIDAD { get; set; }

        [ConcurrencyCheck]
        public int? CANTRESERVADA { get; set; }

        [ConcurrencyCheck]
        public int? CANTPEDIDAPAN { get; set; }

        [ConcurrencyCheck]
        public int? CANTRESERVADAPAN { get; set; }

        [ConcurrencyCheck]
        public int? CANTIDADPAN { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string AGOTADO { get; set; }

        [ConcurrencyCheck]
        public int? CANTPROCESO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int INTENTOS { get; set; }

        [ConcurrencyCheck]
        public string ERROR { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHA_INTEGRA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL639 { get; set; }

    }
}