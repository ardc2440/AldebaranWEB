using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Employee>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Employee?> FindAsync(int employeeId, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task AddAsync(Employee employee, CancellationToken ct = default);
        Task UpdateAsync(int employeeId, Employee employee, CancellationToken ct = default);
        Task DeleteAsync(int employeeId, CancellationToken ct = default);
        Task<IEnumerable<Employee>> GetByAreaAsync(short areaId, CancellationToken ct = default);
        Task<IEnumerable<Employee>> GetByAlarmTypeAsync(short alarmTypeId, CancellationToken ct = default);
    }
}
