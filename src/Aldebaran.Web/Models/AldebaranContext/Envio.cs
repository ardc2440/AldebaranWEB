using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ENVIOS", Schema = "dbo")]
    public partial class Envio
    {
        [Key]
        [Required]
        public int IDENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPEDIDO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime FECHAENVIO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMETODOENV { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string NUMGUIA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REMISION { get; set; }

        [ConcurrencyCheck]
        public string LOGIN { get; set; }

        [ConcurrencyCheck]
        public DateTime? FECHAHORA { get; set; }

        [ConcurrencyCheck]
        public DateTime FECHACREACION { get; set; }

        [ConcurrencyCheck]
        public string TRIAL548 { get; set; }

        public ICollection<Detenvio> Detenvios { get; set; }

        public Metodosenvio Metodosenvio { get; set; }

        public Pedido Pedido { get; set; }

        public ICollection<Subitemdetenvio> Subitemdetenvios { get; set; }

    }
}