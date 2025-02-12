using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IVisualizedAutomaticInProcessAlarmService
    {
        Task AddAsync(VisualizedAutomaticInProcess item, CancellationToken ct = default);
    }
}
