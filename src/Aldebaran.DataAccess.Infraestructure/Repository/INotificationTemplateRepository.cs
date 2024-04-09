using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface INotificationTemplateRepository
    {
        Task<IEnumerable<NotificationTemplate>> GetAsync(CancellationToken ct = default);
        Task<NotificationTemplate?> FindAsync(string name, CancellationToken ct = default);
        Task UpdateAsync(short templateId, NotificationTemplate template, CancellationToken ct = default);
    }
}
