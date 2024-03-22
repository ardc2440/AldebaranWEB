using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IInProcessInventoryReportService
    {
        Task<IEnumerable<InProcessInventoryReport>> GetInProcessInventoryReportDataAsync(CancellationToken ct = default);
    }
}
