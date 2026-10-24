using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class ModifiedPurchaseOrder : ITrackeable
    {
        public int ModifiedPurchaseOrderId { get; set; }
        public int PurchaseOrderId { get; set; }
        public short ModificationReasonId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ModificationDate { get; set; }
        public Employee Employee { get; set; } = null!;
        public ModificationReason ModificationReason { get; set; } = null!;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public ICollection<PurchaseOrderNotification>? PurchaseOrderNotifications { get; set; }
        public ICollection<PurchaseOrderTransitAlarm>? PurchaseOrderTransitAlarms { get; set; }
        
        public ModifiedPurchaseOrder()
        {
            ModificationDate = DateTime.Now;
        }
    }
}
