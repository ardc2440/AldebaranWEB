using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMSXCOLOR", Schema = "dbo")]
    public partial class Itemsxcolor
    {
        [Key]
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REFITEMXCOLOR { get; set; }

        [ConcurrencyCheck]
        public string REFINTITEMXCOLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMCOLOR { get; set; }

        [ConcurrencyCheck]
        public string NOMITEMXCOLORPROV { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [ConcurrencyCheck]
        public string COLOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTPEDIDA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTRESERVADA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTPEDIDAPAN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTRESERVADAPAN { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADPAN { get; set; }

        [ConcurrencyCheck]
        public string ACTIVO { get; set; }

        [ConcurrencyCheck]
        public string AGOTADO { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTPROCESO { get; set; }

        [ConcurrencyCheck]
        public string USAALARMACANTIDADMINIMA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int CANTIDADMINIMAALARMA { get; set; }

        [ConcurrencyCheck]
        public string TRIAL610 { get; set; }

        public ICollection<Alarmascantidadesminima> Alarmascantidadesminimas { get; set; }

        public ICollection<Anuladetcantproceso> Anuladetcantprocesos { get; set; }

        public ICollection<Anulasubitemdetproceso> Anulasubitemdetprocesos { get; set; }

        public ICollection<Anulasubitemdetproceso> Anulasubitemdetprocesos1 { get; set; }

        public ICollection<Detcantproceso> Detcantprocesos { get; set; }

        public ICollection<Detentregaspact> Detentregaspacts { get; set; }

        public ICollection<Detenvio> Detenvios { get; set; }

        public ICollection<Integrasaldo> Integrasaldos { get; set; }

        public ICollection<Itemorden> Itemordens { get; set; }

        public ICollection<Itempedido> Itempedidos { get; set; }

        public ICollection<Itempedidoagotado> Itempedidoagotados { get; set; }

        public ICollection<Itemreserva> Itemreservas { get; set; }

        public ICollection<Itemsxbodega> Itemsxbodegas { get; set; }

        public Item Item { get; set; }

        public ICollection<Itemsxtraslado> Itemsxtraslados { get; set; }

        public ICollection<Itemxitem> Itemxitems { get; set; }

        public ICollection<Itemxitem> Itemxitems1 { get; set; }

        public ICollection<Subitemdetenvio> Subitemdetenvios { get; set; }

        public ICollection<Subitemdetenvio> Subitemdetenvios1 { get; set; }

        public ICollection<Subitemdetproceso> Subitemdetprocesos { get; set; }

        public ICollection<Subitemdetproceso> Subitemdetprocesos1 { get; set; }

    }
}