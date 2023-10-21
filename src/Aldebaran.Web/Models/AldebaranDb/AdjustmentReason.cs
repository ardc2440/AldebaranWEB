using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("adjustment_reasons", Schema = "dbo")]
    public partial class AdjustmentReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short ADJUSTMENT_REASON_ID { get; set; }

        [Required]
        public string ADJUSTMENT_REASON_NAME { get; set; }

        public string ADJUSTMENT_REASON_NOTES { get; set; }

        public ICollection<Adjustment> Adjustments { get; set; }

    }
}