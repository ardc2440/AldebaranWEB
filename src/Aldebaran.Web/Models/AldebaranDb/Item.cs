using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("items", Schema = "dbo")]
    public partial class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ITEM_ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public short LINE_ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string INTERNAL_REFERENCE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ITEM_NAME { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string PROVIDER_REFERENCE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string PROVIDER_ITEM_NAME { get; set; }

        [Required]
        [ConcurrencyCheck]
        public double FOB_COST { get; set; }

        [Required]
        [ConcurrencyCheck]
        public short CURRENCY_ID { get; set; }

        [ConcurrencyCheck]
        public string NOTES { get; set; }

        [ConcurrencyCheck]
        public bool IS_EXTERNAL_INVENTORY { get; set; }

        [Required]
        [ConcurrencyCheck]
        public double CIF_COST { get; set; }

        [ConcurrencyCheck]
        public double? VOLUME { get; set; }

        [ConcurrencyCheck]
        public double? WEIGHT { get; set; }

        [ConcurrencyCheck]
        public short? FOB_MEASURE_UNIT_ID { get; set; }

        [ConcurrencyCheck]
        public short? CIF_MEASURE_UNIT_ID { get; set; }

        [ConcurrencyCheck]
        public bool IS_DOMESTIC_PRODUCT { get; set; }

        [ConcurrencyCheck]
        public bool IS_ACTIVE { get; set; }

        [ConcurrencyCheck]
        public bool IS_CATALOG_VISIBLE { get; set; }

        public ICollection<ItemReference> ItemReferences { get; set; }

        public MeasureUnit MeasureUnit { get; set; }

        public Currency Currency { get; set; }

        public MeasureUnit MeasureUnit1 { get; set; }

        public Line Line { get; set; }

        public ICollection<ItemsArea> ItemsAreas { get; set; }

    }
}