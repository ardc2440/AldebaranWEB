using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetGetByCountryIdAsyncAsync(int countryId, CancellationToken ct = default);
        Task<Department?> FindAsync(int departmentId, CancellationToken ct = default);
    }
}
