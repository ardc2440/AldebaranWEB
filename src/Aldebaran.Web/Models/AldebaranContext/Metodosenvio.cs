using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("METODOSENVIO", Schema = "dbo")]
    public partial class Metodosenvio
    {
        [Key]
        [Required]
        public int IDMETODOENV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMMETODOENV { get; set; }

        [ConcurrencyCheck]
        public string DESCMETODOENV { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public ICollection<Envio> Envios { get; set; }

    }
}