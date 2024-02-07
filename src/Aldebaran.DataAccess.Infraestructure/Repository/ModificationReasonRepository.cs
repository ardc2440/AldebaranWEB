using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ModificationReasonRepository : IModificationReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public ModificationReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ModificationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default)
        {
            return await _context.ModificationReasons.AsNoTracking()
              .Where(i => i.DocumentType.DocumentTypeCode.Contains(documentTypeCode))
              .ToListAsync(ct);
        }
    }

}
