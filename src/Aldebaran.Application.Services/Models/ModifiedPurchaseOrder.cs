namespace Aldebaran.Application.Services.Models
{
    public class ModifiedPurchaseOrder
    {
        public int ModifiedPurchaseOrderId { get; set; }
        public int PurchaseOrderId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public Employee Employee { get; set; }
        public ModificationReason ModificationReason { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public ICollection<PurchaseOrderNotification> PurchaseOrderNotifications { get; set; }
        public ICollection<PurchaseOrderTransitAlarm> PurchaseOrderTransitAlarms { get; set; }

        public ModifiedPurchaseOrder()
        {
            ModificationDate = DateTime.Now;
            Employee = new Employee();
            ModificationReason = new ModificationReason();
            PurchaseOrder = new PurchaseOrder();
            PurchaseOrderNotifications = new List<PurchaseOrderNotification>();
            PurchaseOrderTransitAlarms = new List<PurchaseOrderTransitAlarm>();
        }
    }
}
