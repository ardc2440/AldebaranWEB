using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAdjustmentDetailRepository
    {
        Task<IEnumerable<AdjustmentDetail>> GetAsync(string filter, CancellationToken ct = default);
    }

}
