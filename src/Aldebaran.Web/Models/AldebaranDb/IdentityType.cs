using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("identity_types", Schema = "dbo")]
    public partial class IdentityType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDENTITY_TYPE_ID { get; set; }

        [Required]
        public string IDENTITY_TYPE_CODE { get; set; }

        [Required]
        public string IDENTITY_TYPE_NAME { get; set; }

        public ICollection<Customer> Customers { get; set; }

    }
}