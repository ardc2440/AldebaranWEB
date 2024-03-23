using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IWarehouseStockReportRepository
    {
        Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(CancellationToken ct = default);
    }
}
