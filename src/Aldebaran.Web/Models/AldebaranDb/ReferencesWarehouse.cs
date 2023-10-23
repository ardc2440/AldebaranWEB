using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("references_warehouse", Schema = "dbo")]
    public partial class ReferencesWarehouse
    {
        [Key]
        [Required]
        public int REFERENCE_ID { get; set; }

        [Key]
        [Required]
        public short WAREHOUSE_ID { get; set; }

        [Required]
        public int QUANTITY { get; set; }

        public ItemReference ItemReference { get; set; }

        public Warehouse Warehouse { get; set; }

    }
}