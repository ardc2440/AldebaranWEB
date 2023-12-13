using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IShipmentForwarderAgentMethodService
    {
        Task<ShipmentForwarderAgentMethod?> FindAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default);
        Task<IEnumerable<ShipmentForwarderAgentMethod>> GetAsync(int forwarderAgentId, CancellationToken ct = default);
        Task AddAsync(ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default);
        Task UpdateAsync(int shipmentForwarderAgentMethodId, ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default);
        Task DeleteAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default);
    }
}
