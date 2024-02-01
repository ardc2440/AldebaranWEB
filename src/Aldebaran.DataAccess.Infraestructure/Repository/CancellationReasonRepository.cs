using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class CancellationReasonRepository : ICancellationReasonRepository
    {
        private readonly AldebaranDbContext _context;
        public CancellationReasonRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CancellationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default)
        {
            return await _context.CancellationReasons.AsNoTracking()
              .Where(i => i.DocumentType.DocumentTypeCode.Contains(documentTypeCode))
              .ToListAsync(ct);
        }
    }

}
