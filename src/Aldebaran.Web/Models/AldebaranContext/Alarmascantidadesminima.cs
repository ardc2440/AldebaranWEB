using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ALARMASCANTIDADESMINIMAS", Schema = "dbo")]
    public partial class Alarmascantidadesminima
    {
        [Key]
        [Required]
        public int IDALARMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADMINIMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int DISPONIBLEALARMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAALARMA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL522 { get; set; }

        public Itemsxcolor Itemsxcolor { get; set; }

    }
}