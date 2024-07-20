using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAdjustmentRepository
    {
        Task<Adjustment> AddAsync(Adjustment adjustment, CancellationToken ct = default);
        Task CancelAsync(int adjustmentId, CancellationToken ct = default);
        Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default);
        Task<(IEnumerable<Adjustment>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<Adjustment>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default);

    }

}
