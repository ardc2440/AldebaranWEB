using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedLocalWarehouseAlarmRepository
    {
        Task AddAsync(VisualizedLocalWarehouseAlarm item, CancellationToken ct = default);
    }
}
