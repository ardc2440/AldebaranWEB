using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IWarehouseStockReportService
    {
        Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
