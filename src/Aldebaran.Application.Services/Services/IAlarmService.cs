using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAlarmService
    {
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<String> GetDocumentNumber(int documentId, string documentTypeCode, CancellationToken ct = default);
    }

}
