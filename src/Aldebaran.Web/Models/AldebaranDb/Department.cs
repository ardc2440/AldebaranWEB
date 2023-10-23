using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("departments", Schema = "dbo")]
    public partial class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DEPARTMENT_ID { get; set; }

        [Required]
        public string DEPARTMENT_NAME { get; set; }

        [Required]
        public int COUNTRY_ID { get; set; }

        public ICollection<City> Cities { get; set; }

        public Country Country { get; set; }

    }
}