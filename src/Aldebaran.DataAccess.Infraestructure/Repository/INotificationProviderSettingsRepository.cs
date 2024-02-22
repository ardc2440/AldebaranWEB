using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface INotificationProviderSettingsRepository
    {
        Task<NotificationProviderSetting?> FindAsync(string subject, CancellationToken ct = default);
    }
}
