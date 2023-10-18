using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("forwarder_agents", Schema = "dbo")]
    public partial class ForwarderAgent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FORWARDER_AGENT_ID { get; set; }

        [Required]
        public int FORWARDER_ID { get; set; }

        [Required]
        public string FORWARDER_AGENT_NAME { get; set; }

        [Required]
        public string PHONE1 { get; set; }

        public string PHONE2 { get; set; }

        public string FAX { get; set; }

        [Required]
        public string FORWARDER_AGENT_ADDRESS { get; set; }

        [Required]
        public int CITY_ID { get; set; }

        [Required]
        public string CONTACT { get; set; }

        public string EMAIL1 { get; set; }

        public string EMAIL2 { get; set; }

        public City City { get; set; }

        public Forwarder Forwarder { get; set; }

        public ICollection<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }

    }
}