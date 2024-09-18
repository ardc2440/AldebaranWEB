using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{    
    public class VisualizedMinimumQuantityAlarm : ITrackeable
    {
        public int MinimumQuantityAlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        public Employee Employee { get; set; }

        public VisualizedMinimumQuantityAlarm()
        {
            VisualizedDate = DateTime.Now;
        }
    }
}
