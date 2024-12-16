using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IReferenceMovementReportRepository
    {
        Task<IEnumerable<ReferenceMovementReport>> GetReferenceMovementReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
