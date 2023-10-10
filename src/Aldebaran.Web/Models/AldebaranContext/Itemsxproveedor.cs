using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMSXPROVEEDOR", Schema = "dbo")]
    public partial class Itemsxproveedor
    {
        [Key]
        [Required]
        public int IDITEM { get; set; }

        [Key]
        [Required]
        public int IDPROVEEDOR { get; set; }

        [ConcurrencyCheck]
        public string TRIAL616 { get; set; }

        public Item Item { get; set; }

        public Proveedore Proveedore { get; set; }

    }
}