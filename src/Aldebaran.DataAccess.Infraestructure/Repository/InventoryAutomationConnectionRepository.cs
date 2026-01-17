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
            if (connection.Active)
            {
                var exists = await ExistsActiveAsync(connection.ServerName, connection.PortNumber, connection.DatabaseName, ct);
                if (exists)
                    throw new InvalidOperationException("Ya existe otra conexión Automata activa con el mismo Servidor, Puerto y Base de datos.");
            }

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

            if (active)
            {
                var exists = await ExistsActiveExceptAsync(id, connection.ServerName, connection.PortNumber, connection.DatabaseName, ct);
                if (exists)
                    throw new InvalidOperationException("Ya existe otra conexión Automata activa con el mismo Servidor, Puerto y Base de datos.");
            }

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
            if (connection.Active)
            {
                var exists = await ExistsActiveExceptAsync(connection.InventoryAutomationConnectionId, connection.ServerName, connection.PortNumber, connection.DatabaseName, ct);
                if (exists)
                    throw new InvalidOperationException("Ya existe otra conexión Automata activa con el mismo Servidor, Puerto y Base de datos.");
            }

            await ExecuteCommandAsync(async dbContext =>
            {
                dbContext.InventoryAutomationConnections.Update(connection);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return connection;
        }

        public async Task<bool> ExistsActiveAsync(string serverName, string? portNumber, string databaseName, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.InventoryAutomationConnections
                    .AsNoTracking()
                    .AnyAsync(c => c.Active &&
                                   c.ServerName == serverName &&
                                   c.PortNumber == portNumber &&
                                   c.DatabaseName == databaseName, ct);
            }, ct);
        }

        public async Task<bool> ExistsActiveExceptAsync(int id, string serverName, string? portNumber, string databaseName, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.InventoryAutomationConnections
                    .AsNoTracking()
                    .AnyAsync(c => c.Active &&
                                   c.InventoryAutomationConnectionId != id &&
                                   c.ServerName == serverName &&
                                   c.PortNumber == portNumber &&
                                   c.DatabaseName == databaseName, ct);
            }, ct);
        }
    }
}