using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("shipment_methods", Schema = "dbo")]
    public partial class ShipmentMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short SHIPMENT_METHOD_ID { get; set; }

        [Required]
        public string SHIPMENT_METHOD_NAME { get; set; }

        public string SHIPMENT_METHOD_NOTES { get; set; }

        public ICollection<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }

    }
}