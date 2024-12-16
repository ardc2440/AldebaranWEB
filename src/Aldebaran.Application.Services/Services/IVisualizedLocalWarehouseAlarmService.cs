using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IVisualizedLocalWarehouseAlarmService
    {
        Task AddAsync(VisualizedLocalWarehouseAlarm item, CancellationToken ct = default);
    }
}
