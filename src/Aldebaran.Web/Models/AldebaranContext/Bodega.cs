using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("BODEGAS", Schema = "dbo")]
    public partial class Bodega
    {
        [Key]
        [Required]
        public int IDBODEGA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBODEGA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL528 { get; set; }

        public ICollection<Anuladetcantproceso> Anuladetcantprocesos { get; set; }

        public ICollection<Anulasubitemdetproceso> Anulasubitemdetprocesos { get; set; }

        public ICollection<Detcantproceso> Detcantprocesos { get; set; }

        public ICollection<Detenvio> Detenvios { get; set; }

        public ICollection<Integrasaldo> Integrasaldos { get; set; }

        public ICollection<Itemorden> Itemordens { get; set; }

        public ICollection<Itemsxbodega> Itemsxbodegas { get; set; }

        public ICollection<Subitemdetenvio> Subitemdetenvios { get; set; }

        public ICollection<Subitemdetproceso> Subitemdetprocesos { get; set; }

        public ICollection<Traslado> Traslados { get; set; }

        public ICollection<Traslado> Traslados1 { get; set; }

    }
}