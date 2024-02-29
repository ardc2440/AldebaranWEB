using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmRepository : IAlarmRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default)
        {
            return await _context.Alarms.AsNoTracking()
                            .Include(i => i.AlarmMessage.AlarmType.DocumentType)
                            .Where(i => !_context.VisualizedAlarms.AsNoTracking().Any(j => j.AlarmId == i.AlarmId) &&
                                         _context.UsersAlarmTypes.AsNoTracking().Any(k => k.Visualize && 
                                                                                          k.EmployeeId == employeeId && 
                                                                                          k.AlarmTypeId == i.AlarmMessage.AlarmTypeId))
                            .ToListAsync(ct);
        }
    }

}
