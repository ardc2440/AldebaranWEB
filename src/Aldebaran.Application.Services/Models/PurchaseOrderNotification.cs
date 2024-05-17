using Aldebaran.DataAccess.Enums;

namespace Aldebaran.Application.Services.Models
{
    public class PurchaseOrderNotification
    {
        public int PurchaseOrderNotificationId { get; set; }
        public int ModifiedPurchaseOrderId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotifiedMailList { get; set; }
        public int CustomerOrderId { get; set; }
        public NotificationStatus NotificationState { get; set; }
        public string? NotificationSendingErrorMessage { get; set; }

        // Reverse navigation
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public ModifiedPurchaseOrder ModifiedPurchaseOrder { get; set; } = null!;

        public PurchaseOrderNotification()
        {
            NotifiedMailList = "";
            NotificationDate = DateTime.Now;
        }
    }
}
