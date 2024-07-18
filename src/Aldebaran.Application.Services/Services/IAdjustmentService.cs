using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAdjustmentService
    {
        Task<IEnumerable<Adjustment>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Adjustment>> GetAsync(string filter, CancellationToken ct = default);
        Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default);
        Task<Adjustment> AddAsync(Adjustment adjustment, CancellationToken ct = default);
        Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default);
        Task CancelAsync(int adjustmentId, CancellationToken ct = default);
        Task<(IEnumerable<Adjustment> adjustments, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);

    }
}
