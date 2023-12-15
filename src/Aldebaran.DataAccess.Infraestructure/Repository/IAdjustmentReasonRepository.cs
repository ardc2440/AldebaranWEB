using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAdjustmentReasonRepository
    {
        Task<IEnumerable<AdjustmentReason>> GetAsync(CancellationToken ct = default);
    }

}
