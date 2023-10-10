using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ESTADOINVINICIAL", Schema = "dbo")]
    public partial class Estadoinvinicial
    {
        [Required]
        public int ANNO { get; set; }

        [Required]
        public int SEMESTRE { get; set; }

        public string ESTADO { get; set; }

        public string TRIAL581 { get; set; }

    }
}