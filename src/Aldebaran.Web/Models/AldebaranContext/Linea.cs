using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("LINEAS", Schema = "dbo")]
    public partial class Linea
    {
        [Key]
        [Required]
        public int IDLINEA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CODLINEA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMLINEA { get; set; }

        [ConcurrencyCheck]
        public string DEMONIO { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string TRIAL620 { get; set; }

        public ICollection<Ajustesxitem> Ajustesxitems { get; set; }

        public ICollection<Anuladetcantproceso> Anuladetcantprocesos { get; set; }

        public ICollection<Detcantproceso> Detcantprocesos { get; set; }

        public ICollection<Detenvio> Detenvios { get; set; }

        public ICollection<Itempedido> Itempedidos { get; set; }

        public ICollection<Itempedidoagotado> Itempedidoagotados { get; set; }

        public ICollection<Itemreserva> Itemreservas { get; set; }

        public ICollection<Item> Items { get; set; }

        public ICollection<Itemsxtraslado> Itemsxtraslados { get; set; }

    }
}