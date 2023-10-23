using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("adjustment_types", Schema = "dbo")]
    public partial class AdjustmentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short ADJUSTMENT_TYPE_ID { get; set; }

        [Required]
        public string ADJUSTMENT_TYPE_NAME { get; set; }

        public ICollection<Adjustment> Adjustments { get; set; }

    }
}