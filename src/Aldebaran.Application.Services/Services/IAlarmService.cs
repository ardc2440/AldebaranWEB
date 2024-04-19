using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAlarmService
    {
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<IEnumerable<Alarm>> GetByDocumentIdAsync(int documentTypeId, int documentId, CancellationToken ct = default);
        string GetDocumentNumber(int documentId, string documentTypeCode, CancellationToken ct = default);
        Task<Alarm> AddAsync(Alarm alarm, CancellationToken ct = default);
        Task DisableAsync(int alarmId, CancellationToken ct = default);
    }

}
