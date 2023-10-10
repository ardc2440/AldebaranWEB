using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("RITEMS", Schema = "dbo")]
    public partial class Ritem
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [ConcurrencyCheck]
        public int? IDLINEA { get; set; }

        [ConcurrencyCheck]
        public string REFINTERNA { get; set; }

        [ConcurrencyCheck]
        public string NOMITEM { get; set; }

        [ConcurrencyCheck]
        public string REFPROVEEDOR { get; set; }

        [ConcurrencyCheck]
        public string NOMITEMPROV { get; set; }

        [ConcurrencyCheck]
        public string TIPOITEM { get; set; }

        [ConcurrencyCheck]
        public float? COSTOFOB { get; set; }

        [ConcurrencyCheck]
        public int? IDMONEDA { get; set; }

        [ConcurrencyCheck]
        public string TIPPARTE { get; set; }

        [ConcurrencyCheck]
        public string DETERMINANTE { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string INVENTARIOEXT { get; set; }

        [ConcurrencyCheck]
        public string COSTOCIF { get; set; }

        [ConcurrencyCheck]
        public float? VOLUMEN { get; set; }

        [ConcurrencyCheck]
        public float? PESO { get; set; }

        [ConcurrencyCheck]
        public int? IDUNIDADFOB { get; set; }

        [ConcurrencyCheck]
        public int? IDUNIDADCIF { get; set; }

        [ConcurrencyCheck]
        public string PRODNAC { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string CATALOGOVISIBLE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string ACCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int INTENTOS { get; set; }

        [ConcurrencyCheck]
        public string ERROR { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHA_INTEGRA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL636 { get; set; }

    }
}