using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IActivityTypeService
    {
        Task<ActivityType?> FindAsync(short activityTypeId, CancellationToken ct = default);
    }
}
