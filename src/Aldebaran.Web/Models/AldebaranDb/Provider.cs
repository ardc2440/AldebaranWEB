using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("providers", Schema = "dbo")]
    public partial class Provider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PROVIDER_ID { get; set; }

        [Required]
        public int IDENTITY_TYPE_ID { get; set; }

        [Required]
        public string IDENTITY_NUMBER { get; set; }

        [Required]
        public string PROVIDER_CODE { get; set; }

        [Required]
        public string PROVIDER_NAME { get; set; }

        public string PROVIDER_ADDRESS { get; set; }

        public string PHONE { get; set; }

        public string FAX { get; set; }

        public string EMAIL { get; set; }

        public string CONTACT_PERSON { get; set; }

        [Required]
        public int CITY_ID { get; set; }

        public ICollection<ProviderReference> ProviderReferences { get; set; }

        public City City { get; set; }

        public IdentityType IdentityType { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    }
}