using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IProviderReferenceReportService
    {
        Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
