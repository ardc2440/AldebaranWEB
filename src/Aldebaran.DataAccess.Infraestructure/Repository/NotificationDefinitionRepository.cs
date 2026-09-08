using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class NotificationDefinitionRepository : RepositoryBase<AldebaranDbContext>, INotificationDefinitionRepository
    {
        public NotificationDefinitionRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task<IEnumerable<NotificationDefinition>> GetActiveAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.NotificationDefinitions
                    .AsNoTracking()
                    .OrderBy(x => x.IsActive)
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(int notificationDefinitionId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.NotificationDefinitionRoles
                    .Where(x => x.NotificationDefinitionId == notificationDefinitionId)
                    .Select(x => x.RoleId)
                    .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateLastValidationDateAsync(int notificationDefinitionId, DateTime validationDate, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.NotificationDefinitions
                                .FirstOrDefaultAsync(x => x.NotificationDefinitionId == notificationDefinitionId, ct) ?? throw new KeyNotFoundException($"NotificationDefinition with id {notificationDefinitionId} does not exist.");

                entity.LastValidationDate = validationDate;

                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity)
                        .State = EntityState.Unchanged;

                    throw;
                }
            }, ct);
        }

        public async Task<int> ExecuteValidationQueryAsync(string query, CancellationToken ct = default)
        {
            return await ExecuteScalarAsync(async dbContext =>
            {
                await using var connection = dbContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync(ct);

                await using var command = connection.CreateCommand();
                command.CommandText = query;

                var result = await command.ExecuteScalarAsync(ct);
                if (result == null || result == DBNull.Value)
                    return 0;

                return Convert.ToInt32(result);
            }, ct);
        }
    }
}