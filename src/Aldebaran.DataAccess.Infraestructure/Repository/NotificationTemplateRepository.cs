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

        public async Task<IEnumerable<NotificationTemplate>> GetAsync(CancellationToken ct = default)
        {
            return await _context.NotificationTemplates.AsNoTracking().ToListAsync(ct);
        }

        public async Task UpdateAsync(short templateId, NotificationTemplate template, CancellationToken ct = default)
        {
            var entity = await _context.NotificationTemplates.FirstOrDefaultAsync(f => f.NotificationTemplateId == templateId, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Plantilla de correo con Id {templateId} no encontrada");
            entity.Subject = template.Subject;
            entity.Message = template.Message;
            await _context.SaveChangesAsync(ct);
        }
    }
}
