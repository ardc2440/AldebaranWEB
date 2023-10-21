using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("warehouses", Schema = "dbo")]
    public partial class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short WAREHOUSE_ID { get; set; }

        [Required]
        public string WAREHOUSE_NAME { get; set; }

        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }

        public ICollection<ReferencesWarehouse> ReferencesWarehouses { get; set; }

    }
}