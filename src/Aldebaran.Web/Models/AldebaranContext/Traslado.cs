using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("TRASLADOS", Schema = "dbo")]
    public partial class Traslado
    {
        [Key]
        [Required]
        public int IDTRASLADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDBODEGA { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDBODEGADEST { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string NACIONALIZACION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL646 { get; set; }

        public ICollection<Itemsxtraslado> Itemsxtraslados { get; set; }

        public Bodega Bodega { get; set; }

        public Bodega Bodega1 { get; set; }

    }
}