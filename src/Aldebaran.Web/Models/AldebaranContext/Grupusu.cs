using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("GRUPUSU", Schema = "dbo")]
    public partial class Grupusu
    {
        [Key]
        [Required]
        public int IDGRUPO { get; set; }

        [Key]
        [Required]
        public int IDUSUARIO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL581 { get; set; }

        public Grupo Grupo { get; set; }

        public Usuario Usuario { get; set; }

    }
}