using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IUsersAlarmTypeRepository
    {
        Task AddRangeAsync(IEnumerable<UsersAlarmType> items, CancellationToken ct = default);
        Task DeleteAsync(short alarmTypeId, int employeeId, CancellationToken ct = default);
    }
}
