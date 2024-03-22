using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IInventoryReportService
    {
        Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(CancellationToken ct = default);
    }
}
