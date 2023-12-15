using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class StatusDocumentTypeRepository : IStatusDocumentTypeRepository
    {
        private readonly AldebaranDbContext _context;
        public StatusDocumentTypeRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default)
        {
            return await _context.StatusDocumentTypes.AsNoTracking().FirstOrDefaultAsync(f => f.DocumentTypeId == documentTypeId && f.StatusOrder == order, ct);
        }
    }

}
