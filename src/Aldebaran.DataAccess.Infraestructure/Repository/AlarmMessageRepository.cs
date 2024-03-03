using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class AlarmMessageRepository : IAlarmMessageRepository
    {
        private readonly AldebaranDbContext _context;
        public AlarmMessageRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AlarmMessage>> GetByDocumentTypeIdAsync(short documentTypeId, CancellationToken ct = default)
        {
            return await _context.AlarmMessages.AsNoTracking()
                .Include(i => i.AlarmType.DocumentType)
                .Where(i => i.AlarmType.DocumentTypeId == documentTypeId)                            
                .ToListAsync(ct);                
        }
    }

}
