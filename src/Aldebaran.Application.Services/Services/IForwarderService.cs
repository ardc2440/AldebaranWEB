using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IForwarderService
    {
        Task<IEnumerable<Forwarder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<Forwarder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<Forwarder?> FindAsync(int forwarderId, CancellationToken ct = default);
        Task<bool> ExistsByForwarderName(string forwarderName, CancellationToken ct = default);
        Task AddAsync(Forwarder forwarder, CancellationToken ct = default);
        Task UpdateAsync(int forwarderId, Forwarder forwarder, CancellationToken ct = default);
        Task DeleteAsync(int forwarderId, CancellationToken ct = default);
    }
}
