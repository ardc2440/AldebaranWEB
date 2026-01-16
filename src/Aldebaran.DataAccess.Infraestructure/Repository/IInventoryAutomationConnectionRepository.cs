using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IInventoryAutomationConnectionRepository 
    {
        Task<InventoryAutomationConnection> AddAsync(InventoryAutomationConnection connection, CancellationToken ct = default);

        Task<InventoryAutomationConnection> GetByIdAsync(int Id, CancellationToken ct = default);

        Task<IEnumerable<InventoryAutomationConnection>> GetAllAsync(CancellationToken ct = default);

        Task<InventoryAutomationConnection> UpdateAsync(InventoryAutomationConnection connection, CancellationToken ct = default);

        Task<InventoryAutomationConnection> ChangeActivationAsync(int Id, bool active, CancellationToken ct = default);
                
    }
}