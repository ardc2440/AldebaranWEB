using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("ITEMS", Schema = "dbo")]
    public partial class Item
    {
        [Key]
        [Required]
        public int IDITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDLINEA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REFINTERNA { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string REFPROVEEDOR { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMITEMPROV { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string TIPOITEM { get; set; }

        [Required]
        [ConcurrencyCheck]
        public float COSTOFOB { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDMONEDA { get; set; }

        [ConcurrencyCheck]
        public string TIPPARTE { get; set; }

        [ConcurrencyCheck]
        public string DETERMINANTE { get; set; }

        [ConcurrencyCheck]
        public string OBSERVACIONES { get; set; }

        [Required]
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

        [ConcurrencyCheck]
        public string TRIAL600 { get; set; }

        public ICollection<Ajustesxitem> Ajustesxitems { get; set; }

        public ICollection<Anulasubitemdetproceso> Anulasubitemdetprocesos { get; set; }

        public ICollection<Detcantproceso> Detcantprocesos { get; set; }

        public ICollection<Detenvio> Detenvios { get; set; }

        public ICollection<Embalaje> Embalajes { get; set; }

        public ICollection<Integrasaldo> Integrasaldos { get; set; }

        public ICollection<Itempedido> Itempedidos { get; set; }

        public ICollection<Itempedidoagotado> Itempedidoagotados { get; set; }

        public ICollection<Itemreserva> Itemreservas { get; set; }

        public Linea Linea { get; set; }

        public Moneda Moneda { get; set; }

        public Unidadesmedidum Unidadesmedidum { get; set; }

        public Unidadesmedidum Unidadesmedidum1 { get; set; }

        public ICollection<Itemsxarea> Itemsxareas { get; set; }

        public ICollection<Itemsxcolor> Itemsxcolors { get; set; }

        public ICollection<Itemsxproveedor> Itemsxproveedors { get; set; }

        public ICollection<Itemsxtraslado> Itemsxtraslados { get; set; }

        public ICollection<Subitemdetenvio> Subitemdetenvios { get; set; }

        public ICollection<Subitemdetproceso> Subitemdetprocesos { get; set; }

    }
}