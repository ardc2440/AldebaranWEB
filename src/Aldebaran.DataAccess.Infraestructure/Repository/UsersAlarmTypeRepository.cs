using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class UsersAlarmTypeRepository : IUsersAlarmTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public UsersAlarmTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddRangeAsync(IEnumerable<UsersAlarmType> items, CancellationToken ct = default)
        {
            await _context.UsersAlarmTypes.AddRangeAsync(items, ct);
            await _context.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(short alarmTypeId, int employeeId, CancellationToken ct = default)
        {
            var entity = await _context.UsersAlarmTypes.FirstOrDefaultAsync(x => x.AlarmTypeId == alarmTypeId && x.EmployeeId == employeeId, ct) ?? throw new KeyNotFoundException($"Alarma con id {alarmTypeId} no existe para el empleado {employeeId}.");
            _context.UsersAlarmTypes.Remove(entity);
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }
}
