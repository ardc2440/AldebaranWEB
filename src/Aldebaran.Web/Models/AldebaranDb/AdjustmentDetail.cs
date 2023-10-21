using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("adjustment_details", Schema = "dbo")]
    public partial class AdjustmentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ADJUSTMENT_DETAIL_ID { get; set; }

        [Required]
        public int ADJUSTMENT_ID { get; set; }

        [Required]
        public int REFERENCE_ID { get; set; }

        [Required]
        public short WAREHOUSE_ID { get; set; }

        [Required]
        public int QUANTITY { get; set; }

        public Adjustment Adjustment { get; set; }

        public ItemReference ItemReference { get; set; }

        public Warehouse Warehouse { get; set; }

    }
}