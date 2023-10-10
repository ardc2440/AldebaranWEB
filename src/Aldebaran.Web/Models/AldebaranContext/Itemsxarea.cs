using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMSXAREA", Schema = "dbo")]
    public partial class Itemsxarea
    {
        [Key]
        [Required]
        public int IDITEM { get; set; }

        [Key]
        [Required]
        public int IDAREA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL603 { get; set; }

        public Area Area { get; set; }

        public Item Item { get; set; }

    }
}