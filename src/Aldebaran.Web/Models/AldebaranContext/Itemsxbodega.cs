using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMSXBODEGA", Schema = "dbo")]
    public partial class Itemsxbodega
    {
        [Key]
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [Key]
        [Required]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [ConcurrencyCheck]
        public string TRIAL607 { get; set; }

        public ICollection<Detdevolpedido> Detdevolpedidos { get; set; }

        public Bodega Bodega { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

    }
}