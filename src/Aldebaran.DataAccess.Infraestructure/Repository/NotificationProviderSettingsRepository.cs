using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class NotificationProviderSettingsRepository : RepositoryBase<AldebaranDbContext>, INotificationProviderSettingsRepository
    {
        public NotificationProviderSettingsRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<NotificationProviderSetting?> FindAsync(string subject, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.NotificationProviderSettings.AsNoTracking()
               .FirstOrDefaultAsync(i => i.Subject == subject, ct);
            }, ct);
        }
    }
}
