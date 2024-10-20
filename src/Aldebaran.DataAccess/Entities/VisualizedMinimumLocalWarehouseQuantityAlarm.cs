using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{    
    public class VisualizedMinimumLocalWarehouseQuantityAlarm : ITrackeable
    {
        public int AlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        public Employee Employee { get; set; }

        public VisualizedMinimumLocalWarehouseQuantityAlarm()
        {
            VisualizedDate = DateTime.Now;
        }
    }
}
