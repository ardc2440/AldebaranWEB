using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IInventoryAdjustmentReportService
    {
        Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(CancellationToken ct = default);
    }
}
