using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IEmailNotificationProviderSettingsService
    {
        Task UpdateAsync(string subject, EmailNotificationProvider provider, CancellationToken ct = default);
        Task<EmailNotificationProvider> GetAsync(string subject, CancellationToken ct = default);
    }
}
