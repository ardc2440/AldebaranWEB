using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customer_contacts", Schema = "dbo")]
    public partial class CustomerContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_CONTACT_ID { get; set; }

        [Required]
        public int CUSTOMER_ID { get; set; }

        [Required]
        public string CUSTOMER_CONTACT_NAME { get; set; }

        public string TITLE { get; set; }

        public string PHONE { get; set; }

        public string EMAIL { get; set; }

        public Customer Customer { get; set; }

    }
}