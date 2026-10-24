using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IReferenceMovementReportRepository
    {
        Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(ReferenceMovementReportFilter filter, CancellationToken ct = default);
    }
}
