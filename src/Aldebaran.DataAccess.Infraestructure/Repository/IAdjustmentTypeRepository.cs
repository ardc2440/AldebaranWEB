using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAdjustmentTypeRepository
    {
        Task<IEnumerable<AdjustmentType>> GetAsync(CancellationToken ct = default);
    }

}
