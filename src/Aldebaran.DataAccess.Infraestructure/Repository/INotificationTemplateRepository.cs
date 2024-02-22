using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface INotificationTemplateRepository
    {
        Task<NotificationTemplate?> FindAsync(string name, CancellationToken ct = default);
    }
}
