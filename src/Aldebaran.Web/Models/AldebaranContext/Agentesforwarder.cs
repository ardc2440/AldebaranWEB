using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AGENTESFORWARDER", Schema = "dbo")]
    public partial class Agentesforwarder
    {
        [Key]
        [Required]
        public int IDAGENTEFORWARDER { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDFORWARDER { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMAGENTE { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string TELEFONO1 { get; set; }

        [ConcurrencyCheck]
        public string TELEFONO2 { get; set; }

        [ConcurrencyCheck]
        public string FAX { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string DIRECCION { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDPAIS { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CIUDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string CONTACTO { get; set; }

        [ConcurrencyCheck]
        public string MAIL1 { get; set; }

        [ConcurrencyCheck]
        public string MAIL2 { get; set; }

        [ConcurrencyCheck]
        public string TRIAL512 { get; set; }

        public Forwarder Forwarder { get; set; }

        public Paise Paise { get; set; }

        public ICollection<Embarqueagente> Embarqueagentes { get; set; }

        public ICollection<Ordene> Ordenes { get; set; }

    }
}