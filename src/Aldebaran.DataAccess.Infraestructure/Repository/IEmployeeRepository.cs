using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IEmployeeRepository
    {
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
    }

}
