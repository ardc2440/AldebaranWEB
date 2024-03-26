using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IInventoryAdjustmentReportService
    {
        Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(CancellationToken ct = default);
    }
}
