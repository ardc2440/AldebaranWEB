using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedMinimumQuantityAlarmRepository
    {
        Task AddAsync(VisualizedMinimumQuantityAlarm item, CancellationToken ct = default);
    }
}
