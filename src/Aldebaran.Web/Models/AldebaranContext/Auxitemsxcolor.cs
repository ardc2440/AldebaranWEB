using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranContext
{
    [Table("AUXITEMSXCOLOR", Schema = "dbo")]
    public partial class Auxitemsxcolor
    {
        [Required]
        public int IDITEMXCOLOR { get; set; }

        [Required]
        public int IDITEM { get; set; }

        [Required]
        public string REFITEMXCOLOR { get; set; }

        public string REFINTITEMXCOLOR { get; set; }

        [Required]
        public string NOMCOLOR { get; set; }

        public string NOMITEMXCOLORPROV { get; set; }

        public string OBSERVACIONES { get; set; }

        public string COLOR { get; set; }

        [Required]
        public int CANTPEDIDA { get; set; }

        [Required]
        public int CANTIDAD { get; set; }

        [Required]
        public int CANTRESERVADA { get; set; }

        public string TRIAL525 { get; set; }

    }
}