using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemReferenceRepository : IItemReferenceRepository
    {
        private readonly AldebaranDbContext _context;
        public ItemReferenceRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<ItemReference>> GetAsync(CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .ToListAsync(ct);
        }

        public async Task<List<ItemReference>> GetAsync(string filter, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(filter)
                .ToListAsync(ct);
        }

        public async Task<ItemReference?> FindAsync(int referenceId, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .FirstOrDefaultAsync(i => i.ReferenceId == referenceId);
        }
    }

}
