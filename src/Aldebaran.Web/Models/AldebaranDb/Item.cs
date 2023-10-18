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
        public short LINE_ID { get; set; }

        [Required]
        public string INTERNAL_REFERENCE { get; set; }

        [Required]
        public string ITEM_NAME { get; set; }

        [Required]
        public string PROVIDER_REFERENCE { get; set; }

        [Required]
        public string PROVIDER_ITEM_NAME { get; set; }

        [Required]
        public double FOB_COST { get; set; }

        [Required]
        public short CURRENCY_ID { get; set; }

        public string NOTES { get; set; }

        public bool IS_EXTERNAL_INVENTORY { get; set; }

        [Required]
        public double CIF_COST { get; set; }

        public double? VOLUME { get; set; }

        public double? WEIGHT { get; set; }

        public short? FOB_MEASURE_UNIT_ID { get; set; }

        public short? CIF_MEASURE_UNIT_ID { get; set; }

        public bool IS_DOMESTIC_PRODUCT { get; set; }

        public bool IS_ACTIVE { get; set; }

        public bool IS_CATALOG_VISIBLE { get; set; }

        public ICollection<ItemReference> ItemReferences { get; set; }

        public MeasureUnit MeasureUnit { get; set; }

        public Currency Currency { get; set; }

        public MeasureUnit MeasureUnit1 { get; set; }

        public Line Line { get; set; }

        public ICollection<ItemsArea> ItemsAreas { get; set; }

    }
}