using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAlarmRepository 
    {
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
    }
}
