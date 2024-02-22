using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly AldebaranDbContext _context;
        public NotificationTemplateRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<NotificationTemplate?> FindAsync(string name, CancellationToken ct = default)
        {
            return await _context.NotificationTemplates.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Name == name, ct);
        }
    }
}
