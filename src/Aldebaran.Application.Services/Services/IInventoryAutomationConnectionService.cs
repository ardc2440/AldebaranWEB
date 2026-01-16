using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Services
{
    public interface IInventoryAutomationConnectionService
    {
        Task<InventoryAutomationConnection> CreateAsync(InventoryAutomationConnection model, CancellationToken ct = default);
        Task<InventoryAutomationConnection> UpdateAsync(InventoryAutomationConnection model, CancellationToken ct = default);
        Task<InventoryAutomationConnection> ChangeActivationAsync(int id, bool active, CancellationToken ct = default);
        Task<IEnumerable<InventoryAutomationConnection>> GetAllAsync(CancellationToken ct = default);
        Task<InventoryAutomationConnection> GetByIdAsync(int id, CancellationToken ct = default);
    }
}