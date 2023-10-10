using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ENVIOSCORREO", Schema = "dbo")]
    public partial class Envioscorreo
    {
        [Key]
        [Required]
        public int IDENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string TIPOENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTABLAENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ENVIADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAENVIO { get; set; }

        [ConcurrencyCheck]
        public int? IDFUNCIONARIO { get; set; }

        [ConcurrencyCheck]
        public byte[] ADJUNTO { get; set; }

        [ConcurrencyCheck]
        public string ASUNTO { get; set; }

        [ConcurrencyCheck]
        public byte[] BODYMAIL { get; set; }

        [ConcurrencyCheck]
        public string RECIPIENTS { get; set; }

        [ConcurrencyCheck]
        public string BCCCOPY { get; set; }

        [ConcurrencyCheck]
        public string RECIPIENTNAME { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int ATTEMPS { get; set; }

        [ConcurrencyCheck]
        public string TRIAL554 { get; set; }

        public Funcionario Funcionario { get; set; }

    }
}