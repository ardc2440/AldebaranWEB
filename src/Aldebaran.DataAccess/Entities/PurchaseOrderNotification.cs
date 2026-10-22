using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class PurchaseOrderNotification : NotificationBase, ITrackeable
    {
        public int PurchaseOrderNotificationId { get; set; }
        public int ModifiedPurchaseOrderId { get; set; }
        public int CustomerOrderId { get; set; }

        // Reverse navigation
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public ModifiedPurchaseOrder ModifiedPurchaseOrder { get; set; } = null!;
    }
}
