using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("items_area", Schema = "dbo")]
    public partial class ItemsArea
    {
        [Key]
        [Required]
        public int ITEM_ID { get; set; }

        [Key]
        [Required]
        public short AREA_ID { get; set; }

        public Item Item { get; set; }

    }
}