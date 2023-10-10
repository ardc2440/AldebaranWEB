using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("EMBARQUEAGENTE", Schema = "dbo")]
    public partial class Embarqueagente
    {
        [Key]
        [Required]
        public int IDMETAGENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMETEMBARQUE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDAGENTEFORWARDER { get; set; }

        [ConcurrencyCheck]
        public string TRIAL548 { get; set; }

        public Agentesforwarder Agentesforwarder { get; set; }

        public Metodoembarque Metodoembarque { get; set; }

    }
}