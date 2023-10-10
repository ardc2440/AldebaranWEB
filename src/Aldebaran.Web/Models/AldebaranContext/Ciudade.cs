using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("CIUDADES", Schema = "dbo")]
    public partial class Ciudade
    {
        [Required]
        [ConcurrencyCheck]
        public int IDDEPTO { get; set; }

        [Key]
        [Required]
        public int IDCIUDAD { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string NOMCIUDAD { get; set; }

        [ConcurrencyCheck]
        public string TRIAL535 { get; set; }

        public Departamento Departamento { get; set; }

        public ICollection<Cliente> Clientes { get; set; }

        public ICollection<Forwarder> Forwarders { get; set; }

        public ICollection<Satelite> Satelites { get; set; }

    }
}