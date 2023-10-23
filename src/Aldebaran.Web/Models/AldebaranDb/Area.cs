using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("areas", Schema = "dbo")]
    public partial class Area
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short AREA_ID { get; set; }

        [Required]
        public string AREA_CODE { get; set; }

        [Required]
        public string AREA_NAME { get; set; }

        public string DESCRIPTION { get; set; }

        public ICollection<ItemsArea> ItemsAreas { get; set; }

        public ICollection<User> Users { get; set; }

    }
}