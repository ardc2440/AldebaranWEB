using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderTransitAlarmRepository
    {
        Task AddAsync(PurchaseOrderTransitAlarm item, CancellationToken ct = default);
    }
}
