using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IProcessSatelliteRepository
    {
        Task<ProcessSatellite?> FindAsync(int processSatelliteId, CancellationToken ct = default);
        Task<IEnumerable<ProcessSatellite>> GetAsync(CancellationToken ct = default);
    }

}
