using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedMinimumLocalWarehouseQuantityAlarmRepository
    {
        Task AddAsync(VisualizedMinimumLocalWarehouseQuantityAlarm item, CancellationToken ct = default);
    }
}
