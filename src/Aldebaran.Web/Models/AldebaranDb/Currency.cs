using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("currencies", Schema = "dbo")]
    public partial class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short CURRENCY_ID { get; set; }

        [Required]
        public string CURRENCY_NAME { get; set; }

        public ICollection<Item> Items { get; set; }

    }
}