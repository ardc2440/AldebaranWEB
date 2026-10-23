using Aldebaran.DataAccess.Core;
namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderNotification : NotificationBase, ITrackeable
    {
        public int CustomerOrderNotificationId { get; set; }
	    public short NotificationTemplateId { get; set; }
        public int CustomerOrderId {  get; set; }

        // Reverse navigation
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public NotificationTemplate NotificationTemplate { get; set; } = null!;
    }
}
