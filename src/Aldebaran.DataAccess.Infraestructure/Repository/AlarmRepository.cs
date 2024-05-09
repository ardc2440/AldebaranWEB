using Aldebaran.DataAccess.Entities;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmRepository : IAlarmRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<IEnumerable<Alarm>> GetByDocumentIdAsync(int documentTypeId, int documentId, CancellationToken ct = default)
        {
            return await _context.Alarms.AsNoTracking()
                            .Include(i => i.AlarmMessage.AlarmType.DocumentType)
                            .Where(i => i.DocumentId == documentId && i.AlarmMessage.AlarmType.DocumentTypeId == documentTypeId)
                            .ToListAsync(ct);
        }

        public async Task<Alarm> AddAsync(Alarm item, CancellationToken ct = default)
        {
            try
            {
                await _context.Alarms.AddAsync(item, ct);
                await _context.SaveChangesAsync(ct);
                return item;
            }
            catch (Exception)
            {
                _context.Entry(item).State = EntityState.Unchanged;
                throw;
            }
           
        }

        public async Task DisableAsync(int alarmId, CancellationToken ct = default)
        {
            var entity = await _context.Alarms.FirstOrDefaultAsync(x => x.AlarmId == alarmId, ct) ?? throw new KeyNotFoundException($"Alarma con id {alarmId} no existe.");
            entity.IsActive = false;
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }

}
