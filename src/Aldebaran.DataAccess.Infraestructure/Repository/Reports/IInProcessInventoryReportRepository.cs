using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IInProcessInventoryReportRepository
    {
        Task<IEnumerable<InProcessInventoryReport>> GetInProcessInventoryReportDataAsync(CancellationToken ct = default);
    }
}
