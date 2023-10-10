using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("FESTIVOS", Schema = "dbo")]
    public partial class Festivo
    {
        [Key]
        [Required]
        public DateTime FECHA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL581 { get; set; }

    }
}