using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderTransitAlarmService
    {
        Task AddAsync(PurchaseOrderTransitAlarm item, CancellationToken ct = default);
    }
}
