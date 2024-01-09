using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IActivityTypeRepository
    {
        Task<ActivityType?> FindAsync(short activityTypeId, CancellationToken ct = default);
    }
}
