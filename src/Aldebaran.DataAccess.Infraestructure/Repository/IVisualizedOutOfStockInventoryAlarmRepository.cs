using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedOutOfStockInventoryAlarmRepository
    {
        Task AddAsync(VisualizedOutOfStockInventoryAlarm item, CancellationToken ct = default);
    }
}
