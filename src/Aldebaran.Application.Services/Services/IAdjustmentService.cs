using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAdjustmentService
    {
        Task<IEnumerable<Adjustment>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Adjustment>> GetAsync(string filter, CancellationToken ct = default);
        Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default);
        Task AddAsync(Adjustment adjustment, CancellationToken ct = default);
        Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default);
        Task DeleteAsync(int adjustmentId, CancellationToken ct = default);
    }

}
