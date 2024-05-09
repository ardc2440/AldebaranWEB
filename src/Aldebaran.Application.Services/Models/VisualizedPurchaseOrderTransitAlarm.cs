namespace Aldebaran.Application.Services.Models
{
    public class VisualizedPurchaseOrderTransitAlarm
    {
        public int PurchaseOrderTransitAlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        /*reverse navigation*/
        public PurchaseOrderTransitAlarm PurchaseOrderTransitAlarm { get; set; }
        public Employee Employee { get; set; }

        public VisualizedPurchaseOrderTransitAlarm()
        {
            PurchaseOrderTransitAlarm = new PurchaseOrderTransitAlarm();
            Employee = new Employee();
        }
    }
}
