using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ActivityTypeRepository : RepositoryBase<AldebaranDbContext>, IActivityTypeRepository
    {
        public ActivityTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<ActivityType?> FindAsync(short activityTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ActivityTypes.AsNoTracking()
                    .FirstOrDefaultAsync(i => i.ActivityTypeId == activityTypeId, ct);
            }, ct);
        }
    }

}
