using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("OPCIONESFTPV1", Schema = "dbo")]
    public partial class OpcionesftpV1
    {
        [Key]
        [Required]
        public int IDOPCIONES { get; set; }

        [ConcurrencyCheck]
        public string SERVIDOR { get; set; }

        [ConcurrencyCheck]
        public string USUARIO { get; set; }

        [ConcurrencyCheck]
        public string CLAVE { get; set; }

        [ConcurrencyCheck]
        public string PDFPROMOS { get; set; }

        [ConcurrencyCheck]
        public string DIRPDFPROMOS { get; set; }

        [ConcurrencyCheck]
        public string XLSPROMOS { get; set; }

        [ConcurrencyCheck]
        public string DIRXLSPROMOS { get; set; }

        [ConcurrencyCheck]
        public string DIRHTMLPROMOS { get; set; }

        [ConcurrencyCheck]
        public string PDFGORRAS { get; set; }

        [ConcurrencyCheck]
        public string DIRPDFGORRAS { get; set; }

        [ConcurrencyCheck]
        public string XLSGORRAS { get; set; }

        [ConcurrencyCheck]
        public string DIRXLSGORRAS { get; set; }

        [ConcurrencyCheck]
        public string DIRHTMLGORRAS { get; set; }

        [ConcurrencyCheck]
        public string PDFBOLIG { get; set; }

        [ConcurrencyCheck]
        public string DIRPDFBOLIG { get; set; }

        [ConcurrencyCheck]
        public string XLSBOLIG { get; set; }

        [ConcurrencyCheck]
        public string DIRXLSBOLIG { get; set; }

        [ConcurrencyCheck]
        public string DIRHTMLBOLIG { get; set; }

        [ConcurrencyCheck]
        public string GENERAR { get; set; }

        [ConcurrencyCheck]
        public int? MINUTOS { get; set; }

        [ConcurrencyCheck]
        public string INICIARENSTARTUP { get; set; }

        [ConcurrencyCheck]
        public string DIRXLSINVENTARIO { get; set; }

        [ConcurrencyCheck]
        public string PDFCAMIS { get; set; }

        [ConcurrencyCheck]
        public string DIRPDFCAMIS { get; set; }

        [ConcurrencyCheck]
        public string XLSCAMIS { get; set; }

        [ConcurrencyCheck]
        public string DIRXLSCAMIS { get; set; }

        [ConcurrencyCheck]
        public string DIRHTMLCAMIS { get; set; }

        [ConcurrencyCheck]
        public string TRIAL623 { get; set; }

    }
}