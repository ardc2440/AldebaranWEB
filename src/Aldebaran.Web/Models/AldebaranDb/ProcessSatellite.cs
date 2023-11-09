using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("process_satellites", Schema = "dbo")]

    public class ProcessSatellite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PROCESS_SATELLITE_ID { get; set; }

        [Required]
        public string PROCESS_SATELLITE_NAME { get; set; }

        [Required]
        public string PROCESS_SATELLITE_ADDRESS { get; set; }

        [Required]
        public int IDENTITY_TYPE_ID { get; set; }

        [Required]
        public string IDENTITY_NUMBER { get; set; }

        [Required]
        public string PHONE { get; set; }

        public string FAX { get; set; }

        public string EMAIL { get; set; }

        [Required]
        public int CITY_ID { get; set; }

        [Required]
        public string LEGAL_REPRESENTATIVE { get; set; }

        [Required]
        public bool IS_ACTIVE { get; set; }

        public City City { get; set; }

        public IdentityType IdentityType { get; set; }

        public ICollection<CustomerOrderInProcess> CustomerOrdersInProcess { get; set; }
    }
}
