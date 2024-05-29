using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class UsersAlarmTypeRepository : RepositoryBase<AldebaranDbContext>, IUsersAlarmTypeRepository
    {
        public UsersAlarmTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddRangeAsync(IEnumerable<UsersAlarmType> items, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.UsersAlarmTypes.AddRangeAsync(items, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
        public async Task DeleteAsync(short alarmTypeId, int employeeId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.UsersAlarmTypes.FirstOrDefaultAsync(x => x.AlarmTypeId == alarmTypeId && x.EmployeeId == employeeId, ct) ?? throw new KeyNotFoundException($"Alarma con id {alarmTypeId} no existe para el empleado {employeeId}.");
                dbContext.UsersAlarmTypes.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }
    }
}
