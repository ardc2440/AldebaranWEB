using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("PAISES", Schema = "dbo")]
    public partial class Paise
    {
        [Key]
        [Required]
        public int IDPAIS { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMPAIS { get; set; }

        [ConcurrencyCheck]
        public string TRIAL626 { get; set; }

        public ICollection<Agentesforwarder> Agentesforwarders { get; set; }

    }
}