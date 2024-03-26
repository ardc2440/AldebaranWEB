using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IInventoryReportService
    {
        Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(string referenceIdsFilter, CancellationToken ct = default);
    }
}
