using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("document_types", Schema = "dbo")]
    public partial class DocumentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short DOCUMENT_TYPE_ID { get; set; }

        [Required]
        public string DOCUMENT_TYPE_NAME { get; set; }

        [Required]
        public string DOCUMENT_TYPE_CODE { get; set; }

        [Required]
        public int NEXT_DOCUMENT_NUMBER { get; set; }

        public ICollection<StatusDocumentType> StatusDocumentTypes { get; set; }

    }
}