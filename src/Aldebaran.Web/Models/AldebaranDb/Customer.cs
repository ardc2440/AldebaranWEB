using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("customers", Schema = "dbo")]
    public partial class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CUSTOMER_ID { get; set; }

        [Required]
        public int IDENTITY_TYPE_ID { get; set; }

        [Required]
        public string IDENTITY_NUMBER { get; set; }

        [Required]
        public string CUSTOMER_NAME { get; set; }

        [Required]
        public string PHONE1 { get; set; }

        public string PHONE2 { get; set; }

        public string FAX { get; set; }

        [Required]
        public string CUSTOMER_ADDRESS { get; set; }

        public string CELL_PHONE { get; set; }

        public string EMAIL1 { get; set; }

        public string EMAIL2 { get; set; }

        [Required]
        public int CITY_ID { get; set; }

        public string EMAIL3 { get; set; }

        public bool SEND_EMAIL { get; set; }

        public ICollection<CustomerContact> CustomerContacts { get; set; }

        public City City { get; set; }

        public IdentityType IdentityType { get; set; }

    }
}