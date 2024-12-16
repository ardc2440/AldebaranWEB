using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedPurchaseOrderTransitAlarmRepository
    {
        Task AddAsync(VisualizedPurchaseOrderTransitAlarm item, CancellationToken ct = default);
    }
}
