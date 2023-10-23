using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("adjustments", Schema = "dbo")]
    public partial class Adjustment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ADJUSTMENT_ID { get; set; }

        public DateTime ADJUSTMENT_DATE { get; set; }

        [Required]
        public short ADJUSTMENT_REASON_ID { get; set; }

        [Required]
        public short ADJUSTMENT_TYPE_ID { get; set; }

        [Required]
        public int EMPLOYEE_ID { get; set; }

        public string NOTES { get; set; }

        public DateTime CREATION_DATE { get; set; }

        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }

        public AdjustmentReason AdjustmentReason { get; set; }

        public AdjustmentType AdjustmentType { get; set; }

        public Employee Employee { get; set; }

    }
}