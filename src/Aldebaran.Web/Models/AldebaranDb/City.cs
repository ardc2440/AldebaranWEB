using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("cities", Schema = "dbo")]
    public partial class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CITY_ID { get; set; }

        [Required]
        public string CITY_NAME { get; set; }

        [Required]
        public short DEPARTMENT_ID { get; set; }

        public Department Department { get; set; }

        public ICollection<ForwarderAgent> ForwarderAgents { get; set; }

        public ICollection<Forwarder> Forwarders { get; set; }

    }
}