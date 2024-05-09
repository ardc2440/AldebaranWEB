namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderTransitAlarm
    {
        public int PurchaseOrderTransitAlarmId { get; set; }
        public int ModifiedPurchaseOrderId { get; set; }
        public DateTime OldExpectedReceiptDate { get; set; }

        /*reverse navigation*/
        public ModifiedPurchaseOrder ModifiedPurchaseOrder { get; set; } = null!;
        public ICollection<VisualizedPurchaseOrderTransitAlarm> VisualizedAlarms { get; set; }

        public PurchaseOrderTransitAlarm()
        {
            VisualizedAlarms = new List<VisualizedPurchaseOrderTransitAlarm>();
        }
    }
}
