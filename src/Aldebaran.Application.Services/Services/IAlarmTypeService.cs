using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAlarmTypeService
    {
        Task<IEnumerable<AlarmType>> GetAsync(CancellationToken ct = default);
        Task<AlarmType?> FindAsync(short alarmTypeId, CancellationToken ct = default);
    }
}
