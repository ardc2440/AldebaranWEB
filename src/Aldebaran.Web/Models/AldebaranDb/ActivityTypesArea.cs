using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("activity_types_area", Schema = "dbo")]
    public partial class ActivityTypeArea
    {

        [Key]
        [Required]
        public short ACTIVITY_TYPE_ID { get; set; }

        [Key]
        [Required]
        public short AREA_ID { get; set; }

        public Area Area { get; set; }

        public ActivityType ActivityType { get; set; }
    }
}
