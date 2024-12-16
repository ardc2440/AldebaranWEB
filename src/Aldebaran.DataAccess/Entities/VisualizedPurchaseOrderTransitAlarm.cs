using Aldebaran.DataAccess.Core;

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
