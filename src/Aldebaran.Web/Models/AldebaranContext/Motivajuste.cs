using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MOTIVAJUSTES", Schema = "dbo")]
    public partial class Motivajuste
    {
        [Key]
        [Required]
        public int IDMOTIVAJUSTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMMOTIVAJUSTE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL623 { get; set; }

        public ICollection<Ajuste> Ajustes { get; set; }

    }
}