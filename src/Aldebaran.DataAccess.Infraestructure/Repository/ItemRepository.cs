using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly AldebaranDbContext _context;
        public ItemRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Item?> FindAsync(int itemId, CancellationToken ct = default)
        {
            return await _context.Items.AsNoTracking()
               .Include(i => i.Currency)
               .Include(i => i.Line)
               .Include(i => i.CifMeasureUnit)
               .Include(i => i.FobMeasureUnit)
               .FirstOrDefaultAsync(f => f.ItemId == itemId, ct);
        }

        public async Task<IEnumerable<Item>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Items.AsNoTracking()
                .Include(i => i.Currency)
                .Include(i => i.Line)
                .Include(i => i.CifMeasureUnit)
                .Include(i => i.FobMeasureUnit)
                .ToListAsync(ct);
        }
    }
}
