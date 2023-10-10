using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("MONEDAS", Schema = "dbo")]
    public partial class Moneda
    {
        [Key]
        [Required]
        public int IDMONEDA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMMONEDA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL623 { get; set; }

        public ICollection<Item> Items { get; set; }

    }
}