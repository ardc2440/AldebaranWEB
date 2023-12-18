using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Employee>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Employee?> FindAsync(int employeeId, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task AddAsync(Employee employee, CancellationToken ct = default);
        Task UpdateAsync(int employeeId, Employee employee, CancellationToken ct = default);
        Task DeleteAsync(int employeeId, CancellationToken ct = default);
    }
}
