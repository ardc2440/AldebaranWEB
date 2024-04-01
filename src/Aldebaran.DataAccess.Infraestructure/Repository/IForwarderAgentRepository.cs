using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IForwarderAgentRepository
    {
        Task<ForwarderAgent?> FindAsync(int forwarderAgentId, CancellationToken ct = default);
        Task<bool> ExistsByAgentName(string agentName, CancellationToken ct = default);
        Task<IEnumerable<ForwarderAgent>> GetByForwarderIdAsync(int forwarderId, CancellationToken ct = default);
        Task AddAsync(ForwarderAgent forwarderAgent, CancellationToken ct = default);
        Task UpdateAsync(int forwarderAgentId, ForwarderAgent forwarderAgent, CancellationToken ct = default);
        Task DeleteAsync(int forwarderAgentId, CancellationToken ct = default);
    }
}
