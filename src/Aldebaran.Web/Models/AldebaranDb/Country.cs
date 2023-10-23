using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("countries", Schema = "dbo")]
    public partial class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int COUNTRY_ID { get; set; }

        [Required]
        public string COUNTRY_NAME { get; set; }

        [Required]
        public string COUNTRY_CODE { get; set; }

        public ICollection<Department> Departments { get; set; }

    }
}