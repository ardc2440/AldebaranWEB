using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("OPCIONESSIS", Schema = "dbo")]
    public partial class Opcionessi
    {
        public string VALIDARVERSIONAPLICACION { get; set; }

        [Required]
        public int TIMERREVALARMACANTMINIMA { get; set; }

        [Required]
        public int TIEMPOEJECUCIONAJUSTEINV { get; set; }

        public string TRIAL626 { get; set; }

    }
}