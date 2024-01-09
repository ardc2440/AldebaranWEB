using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IActivityTypesAreaRepository
    {
        Task<IEnumerable<ActivityTypesArea>> GetByAreaAsync(short areaId, CancellationToken ct = default);

        Task<IEnumerable<ActivityTypesArea>> GetByActivityTypeAsync(short activityTypeId, CancellationToken ct = default);
    }
}
