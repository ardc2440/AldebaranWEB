using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IInProcessInventoryReportService
    {
        Task<IEnumerable<InProcessInventoryReport>> GetInProcessInventoryReportDataAsync(string referenceIdsFilter = "", CancellationToken ct = default);
    }
}
