using Aldebaran.Application.Services.Models;
namespace Aldebaran.Application.Services
{
    public interface IForwarderAgentService
    {
        Task<ForwarderAgent?> FindAsync(int forwarderAgentId, CancellationToken ct = default);
        Task<IEnumerable<ForwarderAgent>> GetByForwarderIdAsync(int forwarderId, CancellationToken ct = default);
        Task AddAsync(ForwarderAgent forwarderAgent, CancellationToken ct = default);
        Task UpdateAsync(int forwarderAgentId, ForwarderAgent forwarderAgent, CancellationToken ct = default);
        Task DeleteAsync(int forwarderAgentId, CancellationToken ct = default);
    }
}
