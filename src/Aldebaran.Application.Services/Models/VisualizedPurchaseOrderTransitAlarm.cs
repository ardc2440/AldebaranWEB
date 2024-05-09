namespace Aldebaran.Application.Services.Models
{
    public class VisualizedPurchaseOrderTransitAlarm
    {
        public int PurchaseOrderTransitAlarmId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime VisualizedDate { get; set; }

        /*reverse navigation*/
        public PurchaseOrderTransitAlarm PurchaseOrderTransitAlarm { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
