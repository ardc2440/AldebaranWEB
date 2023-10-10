using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMSXTRASLADO", Schema = "dbo")]
    public partial class Itemsxtraslado
    {
        [Key]
        [Required]
        public int IDITEMSXTRASLADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTRASLADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL616 { get; set; }

        public Item Item { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Linea Linea { get; set; }

        public Traslado Traslado { get; set; }

    }
}