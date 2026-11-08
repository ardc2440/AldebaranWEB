using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IWarehouseStockReportService
    {
        Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(WarehouseStockReportFilter filter, CancellationToken ct = default);
    }
}
