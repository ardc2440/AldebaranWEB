using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class NotificationProviderSettingsRepository : INotificationProviderSettingsRepository
    {
        private readonly AldebaranDbContext _context;
        public NotificationProviderSettingsRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<NotificationProviderSetting?> FindAsync(string subject, CancellationToken ct = default)
        {
            return await _context.NotificationProviderSettings.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Subject == subject, ct);
        }
    }
}
