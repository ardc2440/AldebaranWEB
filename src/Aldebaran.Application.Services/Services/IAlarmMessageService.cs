using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IAlarmMessageService 
    { 
        Task<IEnumerable<AlarmMessage>> GetByDocumentTypeIdAsync(short documentTypeId, CancellationToken ct = default);
    }
}
