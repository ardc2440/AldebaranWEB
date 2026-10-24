using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IMinimumWarehouseStockReportService
    {
        Task<IEnumerable<MinimumWarehouseStockReport>> GetMinimumWarehouseStockReportDataAsync(MinimumWarehouseStockReportFilter filter, CancellationToken ct = default);
    }
}
