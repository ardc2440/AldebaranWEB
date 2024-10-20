using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IVisualizedMinimumLocalWarehouseQuantityAlarmService
    {
        Task AddAsync(VisualizedMinimumLocalWarehouseQuantityAlarm item, CancellationToken ct = default);
    }
}
