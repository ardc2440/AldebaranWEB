using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmRepository : RepositoryBase<AldebaranDbContext>, IAlarmRepository
    {
        public AlarmRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<Alarm>> GetByDocumentIdAsync(int documentTypeId, int documentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Alarms.AsNoTracking()
                                .Include(i => i.AlarmMessage.AlarmType.DocumentType)
                                .Where(i => i.DocumentId == documentId && i.AlarmMessage.AlarmType.DocumentTypeId == documentTypeId)
                                .ToListAsync(ct);
            }, ct);
        }

        public async Task<Alarm> AddAsync(Alarm item, CancellationToken ct = default)
        {
            return await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.Alarms.AddAsync(item, ct);
                    await dbContext.SaveChangesAsync(ct);
                    return item;
                }
                catch (Exception)
                {
                    dbContext.Entry(item).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task DisableAsync(int alarmId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Alarms.FirstOrDefaultAsync(x => x.AlarmId == alarmId, ct) ?? throw new KeyNotFoundException($"Alarma con id {alarmId} no existe.");
                entity.IsActive = false;
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }
    }

}
