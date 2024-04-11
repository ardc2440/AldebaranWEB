using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IUsersAlarmTypeService
    {
        Task AddRangeAsync(IEnumerable<UsersAlarmType> items, CancellationToken ct = default);
        Task DeleteAsync(short alarmTypeId, int employeeId, CancellationToken ct = default);
    }
}
