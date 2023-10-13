using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("item_references", Schema = "dbo")]
    public partial class ItemReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int REFERENCE_ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int ITEM_ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REFERENCE_CODE { get; set; }

        [ConcurrencyCheck]
        public string PROVIDER_REFERENCE_CODE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REFERENCE_NAME { get; set; }

        [ConcurrencyCheck]
        public string PROVIDER_REFERENCE_NAME { get; set; }

        [ConcurrencyCheck]
        public string NOTES { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int INVENTORY_QUANTITY { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int ORDERED_QUANTITY { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int RESERVED_QUANTITY { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int WORK_IN_PROCESS_QUANTITY { get; set; }

        [ConcurrencyCheck]
        public bool? IS_ACTIVE { get; set; }

        [ConcurrencyCheck]
        public bool? IS_SOLD_OUT { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int ALARM_MINIMUM_QUANTITY { get; set; }

        public Item Item { get; set; }

    }
}