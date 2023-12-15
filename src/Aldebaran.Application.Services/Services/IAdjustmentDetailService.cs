using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAdjustmentDetailService
    {
        Task<IEnumerable<AdjustmentDetail>> GetAsync(string filter, CancellationToken ct = default);
        Task<AdjustmentDetail?> FindAsync(int adjustmentDetailId, CancellationToken ct = default);
    }

}
