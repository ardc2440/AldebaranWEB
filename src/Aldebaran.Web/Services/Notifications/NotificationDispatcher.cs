using Aldebaran.Application.Services.Notifications;
using Aldebaran.Application.Services.Notifications.Models;

namespace Aldebaran.Web.Services.Notifications
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly INotificationStore _notificationStore;

        public NotificationDispatcher(INotificationStore notificationStore)
        {
            _notificationStore = notificationStore;
        }

        public Task PublishAsync(IReadOnlyCollection<NotificationEvent> notifications, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            _notificationStore.Replace(notifications);

            return Task.CompletedTask;
        }
    }
}