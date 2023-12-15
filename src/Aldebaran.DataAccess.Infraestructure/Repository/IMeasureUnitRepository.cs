using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IMeasureUnitRepository
    {
        Task<IEnumerable<MeasureUnit>> GetAsync(CancellationToken ct = default);
    }
}
