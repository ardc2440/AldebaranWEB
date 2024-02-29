using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedAlarmRepository 
    {
        Task<VisualizedAlarm> AddAsync(VisualizedAlarm item, CancellationToken ct = default);
    }

}
