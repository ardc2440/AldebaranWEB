using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IInventoryReportRepository
    {
        Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(CancellationToken ct = default);
    }
}
