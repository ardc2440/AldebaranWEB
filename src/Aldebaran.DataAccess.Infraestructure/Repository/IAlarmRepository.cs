using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAlarmRepository 
    {
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);

        Task<Alarm> AddAsync(Alarm item, CancellationToken ct = default);

        Task<Alarm> RemoveAsync(Alarm item, CancellationToken ct = default);
    }
}
