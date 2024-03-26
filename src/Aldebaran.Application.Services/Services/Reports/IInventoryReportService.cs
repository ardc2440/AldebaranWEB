using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IInventoryReportService
    {
        Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(CancellationToken ct = default);
    }
}
