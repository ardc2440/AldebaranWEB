using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("METODOEMBARQUE", Schema = "dbo")]
    public partial class Metodoembarque
    {
        [Key]
        [Required]
        public int IDMETEMBARQUE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMMETEMBARQUE { get; set; }

        [ConcurrencyCheck]
        public string DESCMETEMBARQUE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public ICollection<Embarqueagente> Embarqueagentes { get; set; }

    }
}