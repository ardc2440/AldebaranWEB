using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("EMBALAJE", Schema = "dbo")]
    public partial class Embalaje
    {
        [Key]
        [Required]
        public int IDEMBALAJE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [ConcurrencyCheck]
        public float? PESO { get; set; }

        [ConcurrencyCheck]
        public float? ALTURA { get; set; }

        [ConcurrencyCheck]
        public float? ANCHO { get; set; }

        [ConcurrencyCheck]
        public float? LARGO { get; set; }

        [ConcurrencyCheck]
        public int? CANTIDAD { get; set; }

        [ConcurrencyCheck]
        public string TRIAL548 { get; set; }

        public Item Item { get; set; }

    }
}