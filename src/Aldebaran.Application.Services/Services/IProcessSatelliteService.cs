using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IProcessSatelliteService
    {
        Task<ProcessSatellite?> FindAsync(int processSatelliteId, CancellationToken ct = default);
        Task<IEnumerable<ProcessSatellite>> GetAsync(CancellationToken ct = default);

    }

}
