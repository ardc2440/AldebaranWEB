using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IActivityTypesAreaService
    {
        Task<IEnumerable<ActivityTypesArea>> GetByAreaAsync(short areaId, CancellationToken ct = default);

        Task<IEnumerable<ActivityTypesArea>> GetByActivityTypeAsync(short activityTypeId, CancellationToken ct = default);
    }

}
