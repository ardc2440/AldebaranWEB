using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAreaService
    {
        Task<Area?> FindAsync(short areaId, CancellationToken ct = default);
        Task<IEnumerable<Area>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Area>> GetAsync(string searchKey, CancellationToken ct = default);
    }

}
