using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AREAS", Schema = "dbo")]
    public partial class Area
    {
        [Key]
        [Required]
        public int IDAREA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CODIGO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMBRE { get; set; }

        [ConcurrencyCheck]
        public string DESCRIPCION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL525 { get; set; }

        public ICollection<Actpedido> Actpedidos { get; set; }

        public ICollection<Funcionario> Funcionarios { get; set; }

        public ICollection<Itemsxarea> Itemsxareas { get; set; }

        public ICollection<Tiposactxarea> Tiposactxareas { get; set; }

    }
}