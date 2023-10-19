using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("provider_references", Schema = "dbo")]
    public partial class ProviderReference
    {
        [Key]
        [Required]
        public int REFERENCE_ID { get; set; }

        [Key]
        [Required]
        public int PROVIDER_ID { get; set; }

        public Provider Provider { get; set; }

        public ItemReference ItemReference { get; set; }

    }
}