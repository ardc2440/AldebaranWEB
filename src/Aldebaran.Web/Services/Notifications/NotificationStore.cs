using Aldebaran.Application.Services.Notifications;
using Aldebaran.Application.Services.Notifications.Models;
using System.Collections.Concurrent;

namespace Aldebaran.Web.Services.Notifications
{
    public class NotificationStore : INotificationStore
    {
        private readonly object _sync = new();

        private List<NotificationEvent> _notifications = new();

        public long Version { get; private set; }

        public void Replace(IReadOnlyCollection<NotificationEvent> notifications)
        {
            lock (_sync)
            {
                _notifications = notifications.ToList();

                Version++;
            }
        }

        public IReadOnlyCollection<NotificationEvent> GetNotifications()
        {
            lock (_sync)
            {
                return _notifications.ToList();
            }
        }
    }
}