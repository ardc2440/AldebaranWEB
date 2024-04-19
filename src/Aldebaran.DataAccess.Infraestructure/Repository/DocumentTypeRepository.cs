using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public DocumentTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            return await _context.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeCode == code, ct);
        }
        public DocumentType? FindByCode(string code)
        {
            return _context.DocumentTypes.AsNoTracking().FirstOrDefault(f => f.DocumentTypeCode == code);
        }
    }

}
