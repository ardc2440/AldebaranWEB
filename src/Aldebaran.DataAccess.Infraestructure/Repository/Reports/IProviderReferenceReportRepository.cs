using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IProviderReferenceReportRepository
    {
        Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default);
    }
}
