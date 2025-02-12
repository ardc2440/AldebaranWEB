using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IVisualizedAutomaticInProcessAlarmRepository
    {
        Task AddAsync(VisualizedAutomaticInProcess item, CancellationToken ct = default);
    }
}
