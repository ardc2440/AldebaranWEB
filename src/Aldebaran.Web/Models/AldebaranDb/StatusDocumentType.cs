using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("status_document_types", Schema = "dbo")]
    public partial class StatusDocumentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short STATUS_DOCUMENT_TYPE_ID { get; set; }

        [Required]
        public string STATUS_DOCUMENT_TYPE_NAME { get; set; }

        [Required]
        public short DOCUMENT_TYPE_ID { get; set; }

        [Required]
        public string NOTES { get; set; }

        [Required]
        public bool EDIT_MODE { get; set; }

        [Required]
        public short STATUS_ORDER { get; set; }

        public ICollection<CustomerReservation> CustomerReservations { get; set; }

        public ICollection<CustomerOrder> CustomerOrders { get; set; }

        public DocumentType DocumentType { get; set; }

    }
}
