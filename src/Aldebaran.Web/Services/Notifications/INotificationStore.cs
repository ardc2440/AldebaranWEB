using Aldebaran.Application.Services.Notifications;
using Aldebaran.Application.Services.Notifications.Models;

namespace Aldebaran.Web.Services.Notifications
{
    public interface INotificationStore
    {
        void Replace(IReadOnlyCollection<NotificationEvent> notifications);

        IReadOnlyCollection<NotificationEvent> GetNotifications();
    }
}
