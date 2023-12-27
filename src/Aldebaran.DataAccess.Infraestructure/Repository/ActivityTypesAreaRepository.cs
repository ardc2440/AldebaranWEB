using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ActivityTypesAreaRepository : IActivityTypesAreaRepository
    {
        private readonly AldebaranDbContext _context;
        public ActivityTypesAreaRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ActivityTypesArea>> GetByActivityTypeAsync(short activityTypeId, CancellationToken ct = default)
        {
            return await _context.ActivityTypesAreas.AsNoTracking()
                .Include(i => i.ActivityType)
                .Include(i => i.Area)
                .Where(i => i.ActivityTypeId == activityTypeId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ActivityTypesArea>> GetByAreaAsync(short areaId, CancellationToken ct = default)
        {
            return await _context.ActivityTypesAreas.AsNoTracking()
                .Include(i => i.ActivityType)
                .Include(i => i.Area)
                .Where(i => i.AreaId == areaId)
                .ToListAsync(ct);
        }
    }

}
