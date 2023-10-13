using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("measure_units", Schema = "dbo")]
    public partial class MeasureUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short MEASURE_UNIT_ID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string MEASURE_UNIT_NAME { get; set; }

        public ICollection<Item> Items { get; set; }

        public ICollection<Item> Items1 { get; set; }

    }
}