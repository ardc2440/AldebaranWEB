using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAlarmRepository
    {
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        List<Alarm> GetByEmployeeId(int employeeId);
        Task<IEnumerable<Alarm>> GetByDocumentIdAsync(int documentTypeId, int documentId, CancellationToken ct = default);
        Task<Alarm> AddAsync(Alarm item, CancellationToken ct = default);
        Task DisableAsync(int alarmId, CancellationToken ct = default);
    }
}
