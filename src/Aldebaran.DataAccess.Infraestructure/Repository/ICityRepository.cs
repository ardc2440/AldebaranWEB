using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetByDepartmentIdAsync(int departmentId, CancellationToken ct = default);
        Task<City?> FindAsync(int cityId, CancellationToken ct = default);
    }
}
