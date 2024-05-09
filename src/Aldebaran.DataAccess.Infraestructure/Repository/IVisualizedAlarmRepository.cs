using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedAlarmRepository 
    {
        Task AddAsync(VisualizedAlarm item, CancellationToken ct = default);
    }

}
