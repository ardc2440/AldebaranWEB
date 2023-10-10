using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AUXACTORDEN", Schema = "dbo")]
    public partial class Auxactorden
    {
        public int? IDACTIVIDADORDEN { get; set; }

        public int? IDORDEN { get; set; }

        public DateTime? FECHA { get; set; }

        public string ACTIVIDAD { get; set; }

        public DateTime? FECHACREACION { get; set; }

        public string TRIAL525 { get; set; }

    }
}