using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IReferenceMovementReportService
    {
        Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
