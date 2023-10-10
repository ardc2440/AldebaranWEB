using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("UNIDADESMEDIDA", Schema = "dbo")]
    public partial class Unidadesmedidum
    {
        [Key]
        [Required]
        public int IDUNIDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBRE { get; set; }

        [ConcurrencyCheck]
        public string TRIAL646 { get; set; }

        public ICollection<Item> Items { get; set; }

        public ICollection<Item> Items1 { get; set; }

    }
}