using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ILineRepository
    {
        Task<IEnumerable<Line>> GetAsync(CancellationToken ct = default);
    }
}
