using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IMinimumWarehouseStockReportRepository
    {
        Task<IEnumerable<MinimumWarehouseStockReport>> GetMinimumWarehouseStockReportDataAsync(MinimumWarehouseStockReportFilter filter, CancellationToken ct = default);
    }
}
