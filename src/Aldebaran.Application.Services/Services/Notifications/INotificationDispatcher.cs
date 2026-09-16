using Aldebaran.Application.Services.Notifications.Models;

namespace Aldebaran.Application.Services.Notifications
{
    public interface INotificationDispatcher
    {
        Task PublishAsync(IReadOnlyCollection<NotificationEvent> notifications, CancellationToken ct = default);
    }
}
