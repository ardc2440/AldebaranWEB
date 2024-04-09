namespace Aldebaran.Application.Services
{
    public interface INotificationTemplateService
    {
        Task<IEnumerable<Models.NotificationTemplate>> GetAsync(CancellationToken ct = default);
        Task<Models.NotificationTemplate?> FindAsync(string name, CancellationToken ct = default);
        Task UpdateAsync(short templateId, Models.NotificationTemplate template, CancellationToken ct = default);
    }
}
