using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("activity_types", Schema = "dbo")]
    public partial class ActivityType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short ACTIVITY_TYPE_ID { get; set; }

        [Required]
        public string ACTIVITY_TYPE_NAME { get; set; }

        public ICollection<ActivityTypeArea> ActivityTypesAreas { get; set; }

        public ICollection<CustomerOrderActivityDetail> CustomerOrderActivityDetails { get; set; }
    }
}
