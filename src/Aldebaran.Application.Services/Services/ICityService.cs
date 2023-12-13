using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetAsync(int departmentId, CancellationToken ct = default);
        Task<City?> FindAsync(int cityId, CancellationToken ct = default);
    }
}
