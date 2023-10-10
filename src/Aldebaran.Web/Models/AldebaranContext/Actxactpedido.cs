using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ACTXACTPEDIDO", Schema = "dbo")]
    public partial class Actxactpedido
    {
        [Key]
        [Required]
        public int IDTIPOACTIVIDAD { get; set; }

        [Key]
        [Required]
        public int IDACTPEDIDO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL512 { get; set; }

        public Actpedido Actpedido { get; set; }

        public Tiposactividad Tiposactividad { get; set; }

    }
}