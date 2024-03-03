using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IAlarmMessageRepository    
    {
        Task<IEnumerable<AlarmMessage>> GetByDocumentTypeIdAsync(short documentTypeId, CancellationToken ct = default);
    }
}
