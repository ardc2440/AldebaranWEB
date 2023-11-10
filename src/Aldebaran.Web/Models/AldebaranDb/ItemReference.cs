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
        public int ITEM_ID { get; set; }

        [Required]
        public string REFERENCE_CODE { get; set; }

        public string PROVIDER_REFERENCE_CODE { get; set; }

        [Required]
        public string REFERENCE_NAME { get; set; }

        public string PROVIDER_REFERENCE_NAME { get; set; }

        public string NOTES { get; set; }

        [Required]
        public int INVENTORY_QUANTITY { get; set; }

        [Required]
        public int ORDERED_QUANTITY { get; set; }

        [Required]
        public int RESERVED_QUANTITY { get; set; }

        [Required]
        public int WORK_IN_PROCESS_QUANTITY { get; set; }

        public bool IS_ACTIVE { get; set; }

        public bool IS_SOLD_OUT { get; set; }

        [Required]
        public int ALARM_MINIMUM_QUANTITY { get; set; }

        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }

        public Item Item { get; set; }

        public ICollection<ProviderReference> ProviderReferences { get; set; }

        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public ICollection<ReferencesWarehouse> ReferencesWarehouses { get; set; }

        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }

        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }

    }
}