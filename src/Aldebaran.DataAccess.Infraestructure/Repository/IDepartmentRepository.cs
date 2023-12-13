using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAsync(int countryId, CancellationToken ct = default);
        Task<Department?> FindAsync(int departmentId, CancellationToken ct = default);
    }
}
