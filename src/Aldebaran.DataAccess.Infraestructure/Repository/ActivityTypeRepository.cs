using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ActivityTypeRepository : IActivityTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public ActivityTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ActivityType?> FindAsync(short activityTypeId, CancellationToken ct = default)
        {
            return await _context.ActivityTypes.AsNoTracking()
                .FirstOrDefaultAsync(i => i.ActivityTypeId == activityTypeId, ct);
        }
    }

}
