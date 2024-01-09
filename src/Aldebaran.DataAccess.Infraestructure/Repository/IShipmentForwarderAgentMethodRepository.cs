using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IShipmentForwarderAgentMethodRepository
    {
        Task<ShipmentForwarderAgentMethod?> FindAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default);
        Task<IEnumerable<ShipmentForwarderAgentMethod>> GetByForwarderAgentIdAsync(int forwarderAgentId, CancellationToken ct = default);
        Task AddAsync(ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default);
        Task UpdateAsync(int shipmentForwarderAgentMethodId, ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default);
        Task DeleteAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default);

    }
}
