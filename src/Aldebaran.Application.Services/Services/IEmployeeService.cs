using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
    }

}
