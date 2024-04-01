using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IFreezoneVsAvailableReportService
    {
        Task<IEnumerable<FreezoneVsAvailableReport>> GetFreezoneVsAvailableReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
