using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAdjustmentReasonService
    {
        Task<IEnumerable<AdjustmentReason>> GetAsync(CancellationToken ct = default);
    }

}
