using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ORDENES", Schema = "dbo")]
    public partial class Ordene
    {
        [Key]
        [Required]
        public int IDORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMORDEN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHASOLICITUD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAESTRECIBO { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHAREALRECIBO { get; set; }

        [ConcurrencyCheck]
        public int? IDAGENTEFORWARDER { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFUNCIONARIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPROVEEDOR { get; set; }

        [ConcurrencyCheck]
        public int? IDFORWARDER { get; set; }

        [ConcurrencyCheck]
        public int? IDMETFORWARDER { get; set; }

        [ConcurrencyCheck]
        public string ESTADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDEMPRESA { get; set; }

        [ConcurrencyCheck]
        public string NROIMPORTACION { get; set; }

        [ConcurrencyCheck]
        public string PUERTOEMBARQUE { get; set; }

        [ConcurrencyCheck]
        public string NROPROFORMA { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string FECHAACTPAN { get; set; }

        [ConcurrencyCheck]
        public string TRIAL626 { get; set; }

        public ICollection<Actorden> Actordens { get; set; }

        public ICollection<Devolorden> Devolordens { get; set; }

        public ICollection<Itemorden> Itemordens { get; set; }

        public ICollection<Modordene> Modordenes { get; set; }

        public Agentesforwarder Agentesforwarder { get; set; }

        public Empresa Empresa { get; set; }

        public Funcionario Funcionario { get; set; }

    }
}