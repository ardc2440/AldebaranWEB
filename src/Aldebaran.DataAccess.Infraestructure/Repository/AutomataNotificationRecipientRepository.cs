using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AutomataNotificationRecipientRepository : RepositoryBase<AldebaranDbContext>, IAutomataNotificationRecipientRepository
    {
        public AutomataNotificationRecipientRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<AutomataNotificationRecipient> CreateAsync(AutomataNotificationRecipient entity, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.AutomataNotificationRecipients.AddAsync(entity, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);

            return entity;
        }

        public async Task<IEnumerable<AutomataNotificationRecipient>> GetAllAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AutomataNotificationRecipients.AsNoTracking().ToListAsync(ct);
            }, ct);
        }

        public async Task<AutomataNotificationRecipient> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.AutomataNotificationRecipients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            }, ct) ?? throw new KeyNotFoundException($"AutomataNotificationRecipient with id {id} not found");

            return entity;
        }

        public async Task<AutomataNotificationRecipient> UpdateAsync(AutomataNotificationRecipient entity, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                dbContext.AutomataNotificationRecipients.Update(entity);
                await dbContext.SaveChangesAsync(ct);
            }, ct);

            return entity;
        }

        public async Task<List<string>> GetActiveEmailsByTypeAsync(string notificationType, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
                await dbContext.AutomataNotificationRecipients
                    .AsNoTracking()
                    .Where(w => w.NotificationType == notificationType && (w.IsActive == true))
                    .Select(s => s.Email)
                    .ToListAsync(ct), ct);
        }
    }
}
