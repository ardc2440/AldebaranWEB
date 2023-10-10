using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("FORWARDER", Schema = "dbo")]
    public partial class Forwarder
    {
        [Key]
        [Required]
        public int IDFORWARDER { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMFORWARDER { get; set; }

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

        [ConcurrencyCheck]
        public string MAIL1 { get; set; }

        [ConcurrencyCheck]
        public string MAIL2 { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int IDCIUDAD { get; set; }

        [ConcurrencyCheck]
        public string TRIAL581 { get; set; }

        public ICollection<Agentesforwarder> Agentesforwarders { get; set; }

        public Ciudade Ciudade { get; set; }

    }
}