using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IInventoryAutomationConnectionRepository 
    {
        Task<InventoryAutomationConnection> AddAsync(InventoryAutomationConnection connection, CancellationToken ct = default);
        Task<InventoryAutomationConnection> UpdateAsync(InventoryAutomationConnection connection, CancellationToken ct = default);
        Task<InventoryAutomationConnection> ChangeActivationAsync(int id, bool active, CancellationToken ct = default);
        Task<IEnumerable<InventoryAutomationConnection>> GetAllAsync(CancellationToken ct = default);
        Task<InventoryAutomationConnection> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsActiveAsync(string serverName, string? portNumber, string databaseName, CancellationToken ct = default);
        Task<bool> ExistsActiveExceptAsync(int id, string serverName, string? portNumber, string databaseName, CancellationToken ct = default);
    }
}