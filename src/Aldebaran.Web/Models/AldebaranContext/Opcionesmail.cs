using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("OPCIONESMAIL", Schema = "dbo")]
    public partial class Opcionesmail
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string HOST { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int PUERTO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string USUARIO { get; set; }

        [ConcurrencyCheck]
        public string CORREOSALIDA { get; set; }

        [ConcurrencyCheck]
        public string NOMBRESALIDA { get; set; }

        [ConcurrencyCheck]
        public string OBJETOCORREO { get; set; }

        [ConcurrencyCheck]
        public string CONTENIDOCORREO { get; set; }

        [ConcurrencyCheck]
        public string ENVIODECOPIA { get; set; }

        [ConcurrencyCheck]
        public string CLAVE { get; set; }

        [ConcurrencyCheck]
        public string OBJETOCORREORESERVA { get; set; }

        [ConcurrencyCheck]
        public string CONTENIDOCORREORESERVA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int TIEMPOESPERASINCEXIST { get; set; }

        [ConcurrencyCheck]
        public string CORREOSINCEXISTENCIAS { get; set; }

        [ConcurrencyCheck]
        public string USUARIOEXISTENCIAS { get; set; }

        [ConcurrencyCheck]
        public string PASSWORDEXISTENCIAS { get; set; }

        [ConcurrencyCheck]
        public string SUBIREXISTENCIAS { get; set; }

        [ConcurrencyCheck]
        public string OBJETOCORREOAGOTADOS { get; set; }

        [ConcurrencyCheck]
        public string CONTENIDOCORREOAGOTADO { get; set; }

        [ConcurrencyCheck]
        public string OBJETOCORREOPEDANULADO { get; set; }

        [ConcurrencyCheck]
        public string CONTENIDOCORREOPEDANULADO { get; set; }

        [ConcurrencyCheck]
        public string OBJETOCORREORESANULADO { get; set; }

        [ConcurrencyCheck]
        public string CONTENIDOCORREORESANULADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int TIEMPOENVIOCORREO { get; set; }

        [ConcurrencyCheck]
        public string INICIOMONITORAUTOMATICO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL626 { get; set; }

    }
}