using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IVisualizedOutOfStockInventoryAlarmService
    {
        Task AddAsync(VisualizedOutOfStockInventoryAlarm item, CancellationToken ct = default);
    }
}
