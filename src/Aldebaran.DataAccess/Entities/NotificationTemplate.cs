namespace Aldebaran.DataAccess.Entities
{
    public class NotificationTemplate
    {
        public short NotificationTemplateId { get; set; }
        public required string Name { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }

        public ICollection<CustomerOrderNotification> CustomerOrderNotifications { get; set; } = null!;
        public ICollection<CustomerReservationNotification> CustomerReservationNotifications { get; set; } = null!;
    }
}
