using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AldebaranDbContext _context;
        public EmployeeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Employee employee, CancellationToken ct = default)
        {
            await _context.Employees.AddAsync(employee, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int employeeId, CancellationToken ct = default)
        {
            var entity = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct) ?? throw new KeyNotFoundException($"Empleado con id {employeeId} no existe.");
            _context.Employees.Remove(entity);
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

        public async Task<Employee?> FindAsync(int employeeId, CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking()
               .Include(i => i.Area)
               .Include(i => i.IdentityType)
               .FirstOrDefaultAsync(w => w.EmployeeId == employeeId, ct);
        }

        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking()
                .Include(i => i.Area)
                .Include(i => i.IdentityType)
                .FirstOrDefaultAsync(f => f.LoginUserId == loginUserId, ct);
        }

        public async Task<IEnumerable<Employee>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking()
              .Include(i => i.Area)
              .Include(i => i.IdentityType)
              .ToListAsync(ct);
        }

        public async Task<IEnumerable<Employee>> GetByAreaAsync(short areaId, CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking()
              .Include(i => i.Area)
              .Include(i => i.IdentityType)
              .Where(i => i.AreaId == areaId)
              .ToListAsync(ct);
        }

        public async Task<IEnumerable<Employee>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Employees.AsNoTracking()
               .Where(f => f.IdentityNumber.Contains(searchKey) || f.DisplayName.Contains(searchKey) || f.FullName.Contains(searchKey) || f.Position.Contains(searchKey) || f.Area.AreaCode.Contains(searchKey) || f.Area.AreaName.Contains(searchKey) || f.Area.Description.Contains(searchKey))
               .Include(i => i.Area)
               .Include(i => i.IdentityType)
               .ToListAsync(ct);
        }

        public async Task UpdateAsync(int employeeId, Employee employee, CancellationToken ct = default)
        {
            var entity = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct) ?? throw new KeyNotFoundException($"Empleado con id {employeeId} no existe.");
            entity.AreaId = employee.AreaId;
            entity.IdentityTypeId = employee.IdentityTypeId;
            entity.IdentityNumber = employee.IdentityNumber;
            entity.DisplayName = employee.DisplayName;
            entity.FullName = employee.FullName;
            entity.LoginUserId = employee.LoginUserId;
            entity.Position = employee.Position;
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Employee>> GetByAlarmTypeAsync(short alarmTypeId, CancellationToken ct = default)
        {
            return await _context.UsersAlarmTypes.AsNoTracking()
                .Include(i => i.Employee)
                .Where(w => w.AlarmTypeId == alarmTypeId)
                .Select(s => s.Employee)
                .ToListAsync(ct);
        }
    }
}
