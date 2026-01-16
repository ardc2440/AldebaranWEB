using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class FtpWritingConnectionRepository : RepositoryBase<AldebaranDbContext>, IFtpWritingConnectionRepository
    {
        public FtpWritingConnectionRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<FtpWritingConnection> AddAsync(FtpWritingConnection connection, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.FtpWritingConnections.AddAsync(connection, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return connection;
        }
        public async Task<FtpWritingConnection> UpdateAsync(FtpWritingConnection connection, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                dbContext.FtpWritingConnections.Update(connection);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
            return connection;
        }

        public async Task<FtpWritingConnection> ChangeActivationAsync(int Id, bool active, CancellationToken ct = default)
        {
            var connection = await GetByIdAsync(Id, ct) ?? throw new KeyNotFoundException($"conexion con id {Id} no existe");
            connection.Active = active; 
            return await UpdateAsync(connection, ct);
        }

        public async Task<IEnumerable<FtpWritingConnection>> GetAllAsync(CancellationToken ct = default)
        {
            var connections = await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.FtpWritingConnections.AsNoTracking().ToListAsync(ct);
            }, ct);
            return connections;
        }

        public async Task<FtpWritingConnection> GetByIdAsync(int Id, CancellationToken ct = default)
        {
            var connection = await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.FtpWritingConnections.AsNoTracking().FirstOrDefaultAsync(c => c.FtpWritingConnectionId == Id, ct);
            }, ct) ?? throw new KeyNotFoundException($"Conexion con id {Id} no existe");
            return connection;
        }
    }        
}