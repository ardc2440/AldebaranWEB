using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IInventoryAdjustmentReportRepository
    {
        Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(InventoryAdjustmentReportFilter filter, CancellationToken ct = default);
    }
}
