using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ActivityTypesAreaRepository : RepositoryBase<AldebaranDbContext>, IActivityTypesAreaRepository
    {
        public ActivityTypesAreaRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<ActivityTypesArea>> GetByActivityTypeAsync(short activityTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ActivityTypesAreas.AsNoTracking()
                .Include(i => i.ActivityType)
                .Include(i => i.Area)
                .Where(i => i.ActivityTypeId == activityTypeId)
                .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<ActivityTypesArea>> GetByAreaAsync(short areaId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ActivityTypesAreas.AsNoTracking()
                .Include(i => i.ActivityType)
                .Include(i => i.Area)
                .Where(i => i.AreaId == areaId)
                .ToListAsync(ct);
            }, ct);
        }
    }
}
