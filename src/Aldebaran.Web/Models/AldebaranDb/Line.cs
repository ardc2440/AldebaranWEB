using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("lines", Schema = "dbo")]
    public partial class Line
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short LINE_ID { get; set; }

        [Required]
        public string LINE_CODE { get; set; }

        [Required]
        public string LINE_NAME { get; set; }

        public bool IS_DEMON { get; set; }

        public bool IS_ACTIVE { get; set; }

        public ICollection<Item> Items { get; set; }

    }
}