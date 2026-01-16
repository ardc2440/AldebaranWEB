using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class InventoryAutomationConnectionRepository : RepositoryBase<AldebaranDbContext>, IInventoryAutomationConnectionRepository
    {
        public InventoryAutomationConnectionRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<InventoryAutomationConnection> AddAsync(InventoryAutomationConnection connection, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.InventoryAutomationConnections.AddAsync(connection, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return connection;
        }

        public async Task<InventoryAutomationConnection> ChangeActivationAsync(int id, bool active, CancellationToken ct = default)
        {
            var connection = await GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"conexion con id {id} no existe");
            connection.Active = active;
            return await UpdateAsync(connection, ct);
        }

        public async Task<IEnumerable<InventoryAutomationConnection>> GetAllAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.InventoryAutomationConnections.ToListAsync(ct);
            }, ct);
        }

        public async Task<InventoryAutomationConnection> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.InventoryAutomationConnections.FindAsync(new object[] { id }, ct);
            }, ct) ?? throw new KeyNotFoundException($"Conexion con id {id} no existe");
        }

        public async Task<InventoryAutomationConnection> UpdateAsync(InventoryAutomationConnection connection, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                dbContext.InventoryAutomationConnections.Update(connection);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return connection;
        }
    }
}