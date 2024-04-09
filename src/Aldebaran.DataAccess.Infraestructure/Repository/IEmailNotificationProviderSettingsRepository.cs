using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IEmailNotificationProviderSettingsRepository
    {
        Task UpdateAsync(string subject, EmailNotificationProvider provider, CancellationToken ct = default);
        Task<EmailNotificationProvider> GetAsync(string subject, CancellationToken ct = default);
    }
}
