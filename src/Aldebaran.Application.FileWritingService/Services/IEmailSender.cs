using System.Threading;

namespace Aldebaran.Application.FileWritingService.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string[] recipients, string subject, string body, CancellationToken ct = default);
    }
}
