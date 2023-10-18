using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("forwarders", Schema = "dbo")]
    public partial class Forwarder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FORWARDER_ID { get; set; }

        [Required]
        public string FORWARDER_NAME { get; set; }

        [Required]
        public string PHONE1 { get; set; }

        public string PHONE2 { get; set; }

        public string FAX { get; set; }

        [Required]
        public string FORWARDER_ADDRESS { get; set; }

        public string MAIL1 { get; set; }

        public string MAIL2 { get; set; }

        [Required]
        public int CITY_ID { get; set; }

        public ICollection<ForwarderAgent> ForwarderAgents { get; set; }

        public City City { get; set; }

    }
}