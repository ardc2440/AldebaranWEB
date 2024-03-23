
using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IWarehouseStockReportService
    {
        Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(CancellationToken ct = default);
    }
}
