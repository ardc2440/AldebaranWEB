using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMXITEM", Schema = "dbo")]
    public partial class Itemxitem
    {
        [Key]
        [Required]
        public int IDITEMXITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPRODUCTO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDSUBPRODUCTO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

        public Itemsxcolor Itemsxcolor1 { get; set; }

    }
}