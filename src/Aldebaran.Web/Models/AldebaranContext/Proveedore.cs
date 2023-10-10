using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("PROVEEDORES", Schema = "dbo")]
    public partial class Proveedore
    {
        [Key]
        [Required]
        public int IDPROVEEDOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDTIPIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NUMIDENTIFICA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CODPROVEEDOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMPROVEEDOR { get; set; }

        [ConcurrencyCheck]
        public string DIRECCION { get; set; }

        [ConcurrencyCheck]
        public string TELEFONO { get; set; }

        [ConcurrencyCheck]
        public string FAX { get; set; }

        [ConcurrencyCheck]
        public string MAIL { get; set; }

        [ConcurrencyCheck]
        public string CONTACTO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL636 { get; set; }

        public ICollection<Itemsxproveedor> Itemsxproveedors { get; set; }

        public Tipidentifica Tipidentifica { get; set; }

    }
}