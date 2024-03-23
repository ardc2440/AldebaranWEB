using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IProviderReferenceReportService
    {
        Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(CancellationToken ct = default);
    }
}
