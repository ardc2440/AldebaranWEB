using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAlarmTypeRepository
    {
        Task<IEnumerable<AlarmType>> GetAsync(CancellationToken ct = default);
        Task<AlarmType?> FindAsync(short alarmTypeId, CancellationToken ct = default);
    }
}
