using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("TEMP_CUADREINVENTARIOS", Schema = "dbo")]
    public partial class TempCuadreinventario
    {
        [Key]
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public int? PEDIDOINV { get; set; }

        [ConcurrencyCheck]
        public int? COMPROMETIDO { get; set; }

        [ConcurrencyCheck]
        public int? CANTPROCESO { get; set; }

        [ConcurrencyCheck]
        public int? PROCESO { get; set; }

        [ConcurrencyCheck]
        public int? PEDIDO { get; set; }

        [ConcurrencyCheck]
        public int? ENTREGADO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL643 { get; set; }

    }
}