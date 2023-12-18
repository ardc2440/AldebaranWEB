using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAdjustmentDetailRepository
    {
        Task<IEnumerable<AdjustmentDetail>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<AdjustmentDetail?> FindAsync(int adjustmentDetailId, CancellationToken ct = default);
        Task<IEnumerable<AdjustmentDetail>> GetByAdjustmentIdAsync(int adjustmentId, CancellationToken ct = default);
    }

}
