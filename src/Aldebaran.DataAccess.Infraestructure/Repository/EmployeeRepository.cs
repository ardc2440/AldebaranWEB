using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class EmployeeRepository : RepositoryBase<AldebaranDbContext>, IEmployeeRepository
    {
        public EmployeeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(Employee employee, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.Employees.AddAsync(employee, ct);
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }

        public async Task DeleteAsync(int employeeId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct) ?? throw new KeyNotFoundException($"Empleado con id {employeeId} no existe.");
                dbContext.Employees.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task<Employee?> FindAsync(int employeeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Employees.AsNoTracking()
              .Include(i => i.Area)
              .Include(i => i.IdentityType)
              .FirstOrDefaultAsync(w => w.EmployeeId == employeeId, ct);
            }, ct);
        }

        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Employees.AsNoTracking()
                            .Include(i => i.Area)
                            .Include(i => i.IdentityType)
                            .FirstOrDefaultAsync(f => f.LoginUserId == loginUserId, ct);
            }, ct);
        }

        public async Task<IEnumerable<Employee>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Employees.AsNoTracking()
                    .Include(i => i.Area)
                    .Include(i => i.IdentityType)
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<Employee>> GetByAreaAsync(short areaId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Employees.AsNoTracking()
                          .Include(i => i.Area)
                          .Include(i => i.IdentityType)
                          .Where(i => i.AreaId == areaId)
                          .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<Employee>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Employees.AsNoTracking()
                           .Where(f => f.IdentityNumber.Contains(searchKey) || f.DisplayName.Contains(searchKey) || f.FullName.Contains(searchKey) || f.Position.Contains(searchKey) || f.Area.AreaCode.Contains(searchKey) || f.Area.AreaName.Contains(searchKey) || f.Area.Description.Contains(searchKey))
                           .Include(i => i.Area)
                           .Include(i => i.IdentityType)
                           .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int employeeId, Employee employee, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct) ?? throw new KeyNotFoundException($"Empleado con id {employeeId} no existe.");
                entity.AreaId = employee.AreaId;
                entity.IdentityTypeId = employee.IdentityTypeId;
                entity.IdentityNumber = employee.IdentityNumber;
                entity.DisplayName = employee.DisplayName;
                entity.FullName = employee.FullName;
                entity.LoginUserId = employee.LoginUserId;
                entity.Position = employee.Position;
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }

        public async Task<IEnumerable<Employee>> GetByAlarmTypeAsync(short alarmTypeId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.UsersAlarmTypes.AsNoTracking()
                            .Include(i => i.Employee)
                            .Where(w => w.AlarmTypeId == alarmTypeId)
                            .Select(s => s.Employee)
                            .ToListAsync(ct);
            }, ct);
        }
    }
}
