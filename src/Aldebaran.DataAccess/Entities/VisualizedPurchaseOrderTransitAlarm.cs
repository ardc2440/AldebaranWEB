using Aldebaran.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.DataAccess.Entities
{
    public class VisualizedPurchaseOrderTransitAlarm : ITrackeable
    {
        public int PurchaseOrderTransitAlarmId { get; set; }
        public int EmployeeId { get; set; } 
        public DateTime	VisualizedDate { get; set; }

        /*reverse navigation*/

        public PurchaseOrderTransitAlarm PurchaseOrderTransitAlarm {  get; set; }
        public Employee Employee { get; set; }

        public VisualizedPurchaseOrderTransitAlarm() 
        { 
            VisualizedDate = System.DateTime.Now;
        }
    }
}
