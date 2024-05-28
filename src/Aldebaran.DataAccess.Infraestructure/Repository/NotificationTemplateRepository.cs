using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class NotificationTemplateRepository : RepositoryBase<AldebaranDbContext>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<NotificationTemplate?> FindAsync(string name, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.NotificationTemplates.AsNoTracking()
               .FirstOrDefaultAsync(i => i.Name == name, ct);
            }, ct);
        }

        public async Task<IEnumerable<NotificationTemplate>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.NotificationTemplates.AsNoTracking().ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(short templateId, NotificationTemplate template, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.NotificationTemplates.FirstOrDefaultAsync(f => f.NotificationTemplateId == templateId, ct);
                if (entity == null)
                    throw new KeyNotFoundException($"Plantilla de correo con Id {templateId} no encontrada");
                entity.Subject = template.Subject;
                entity.Message = template.Message;
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
