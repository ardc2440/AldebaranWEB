using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAreaRepository
    {
        Task<Area?> FindAsync(short areaId, CancellationToken ct = default);
        Task<IEnumerable<Area>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Area>> GetAsync(string searchKey, CancellationToken ct = default);
    }

}
