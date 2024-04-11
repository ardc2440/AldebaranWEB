using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmTypeRepository : IAlarmTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<AlarmType>> GetAsync(CancellationToken ct = default)
        {
            return await _context.AlarmTypes.AsNoTracking()
                .Include(i => i.DocumentType)
                .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.IdentityType)
                .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.Area)
                .ToListAsync(ct);
        }
        public async Task<AlarmType?> FindAsync(short alarmTypeId, CancellationToken ct = default)
        {
            return await _context.AlarmTypes.AsNoTracking()
                .Include(i => i.DocumentType)
                .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.IdentityType)
                .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.Area)
                .FirstOrDefaultAsync(f => f.AlarmTypeId == alarmTypeId, ct);
        }
    }
}
