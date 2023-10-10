using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AUXORDENES", Schema = "dbo")]
    public partial class Auxordene
    {
        public int? IDORDEN { get; set; }

        public string NUMORDEN { get; set; }

        public DateTime? FECHASOLICITUD { get; set; }

        public DateTime? FECHAESTRECIBO { get; set; }

        public DateTime? FECHAREALRECIBO { get; set; }

        public int? IDAGENTEFORWARDER { get; set; }

        public int? IDFUNCIONARIO { get; set; }

        public int? IDPROVEEDOR { get; set; }

        public int? IDFORWARDER { get; set; }

        public int? IDMETFORWARDER { get; set; }

        public string ESTADO { get; set; }

        public int? IDEMPRESA { get; set; }

        public string NROIMPORTACION { get; set; }

        public string PUERTOEMBARQUE { get; set; }

        public string NROPROFORMA { get; set; }

        public DateTime? FECHACREACION { get; set; }

        public string TRIAL528 { get; set; }

    }
}