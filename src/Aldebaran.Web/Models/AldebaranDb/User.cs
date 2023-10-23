using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("employees", Schema = "dbo")]
    public partial class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EMPLOYEE_ID { get; set; }

        [Required]
        public short AREA_ID { get; set; }

        [Required]
        public int IDENTITY_TYPE_ID { get; set; }

        [Required]
        public string IDENTITY_NUMBER { get; set; }

        public string DISPLAY_NAME { get; set; }

        [Required]
        public string FULL_NAME { get; set; }

        [Required]
        public string LOGIN_USER_ID { get; set; }

        public string POSITION { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        public Area Area { get; set; }

        public IdentityType IdentityType { get; set; }

    }
}