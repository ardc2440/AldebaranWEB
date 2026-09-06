using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class EmployeePreference : ITrackeable
    {
        public int EmployeeId { get; set; }
        public bool EnableNotifications { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Employee Employee { get; set; } = new Employee();
    }
}
