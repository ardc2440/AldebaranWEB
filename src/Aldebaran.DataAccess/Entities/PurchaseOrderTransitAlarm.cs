using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderTransitAlarm
    {
        public int PurchaseOrderTransitAlarmId { get; set; }
	    public int ModifiedPurchaseOrderId {  get; set; }
        public DateTime OldExpectedReceiptDate {  get; set; }
        
        /*reverse navigation*/
        public ModifiedPurchaseOrder ModifiedPurchaseOrder { get; set; }
        public ICollection<VisualizedPurchaseOrderTransitAlarm> VisualizedAlarms { get; set; }

        public PurchaseOrderTransitAlarm()
        {
            VisualizedAlarms = new List<VisualizedPurchaseOrderTransitAlarm>();
        }
    }
}
