using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IVisualizedPurchaseOrderTransitAlarmService
    {
        Task AddAsync(VisualizedPurchaseOrderTransitAlarm item, CancellationToken ct = default);
    }
}
