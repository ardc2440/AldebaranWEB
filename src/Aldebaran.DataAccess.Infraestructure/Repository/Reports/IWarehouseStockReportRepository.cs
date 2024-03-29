using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IWarehouseStockReportRepository
    {
        Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
