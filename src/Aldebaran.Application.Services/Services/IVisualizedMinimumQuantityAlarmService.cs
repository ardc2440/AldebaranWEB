using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IVisualizedMinimumQuantityAlarmService
    {
        Task AddAsync(VisualizedMinimumQuantityAlarm item, CancellationToken ct = default);
    }
}
