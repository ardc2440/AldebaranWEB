using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmTypeRepository : RepositoryBase<AldebaranDbContext>, IAlarmTypeRepository
    {
        public AlarmTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public async Task<IEnumerable<AlarmType>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AlarmTypes.AsNoTracking()
                    .Include(i => i.DocumentType)
                    .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.IdentityType)
                    .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.Area)
                    .ToListAsync(ct);
            }, ct);
        }
        public async Task<AlarmType?> FindAsync(short alarmTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AlarmTypes.AsNoTracking()
                    .Include(i => i.DocumentType)
                    .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.IdentityType)
                    .Include(i => i.UsersAlarmTypes).ThenInclude(i => i.Employee.Area)
                    .FirstOrDefaultAsync(f => f.AlarmTypeId == alarmTypeId, ct);
            }, ct);
        }
    }
}
