using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IInventoryAdjustmentReportRepository
    {
        Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(CancellationToken ct = default);
    }
}
