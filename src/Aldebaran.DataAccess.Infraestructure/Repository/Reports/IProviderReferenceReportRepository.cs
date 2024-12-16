using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IProviderReferenceReportRepository
    {
        Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
