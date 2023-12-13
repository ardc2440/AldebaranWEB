using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemAreaRepository : IItemAreaRepository
    {
        private readonly AldebaranDbContext _context;
        public ItemAreaRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ItemsArea item, CancellationToken ct = default)
        {
            await _context.ItemsAreas.AddAsync(item, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(short areaId, int itemId, CancellationToken ct = default)
        {
            var entity = await _context.ItemsAreas.FirstOrDefaultAsync(x => x.AreaId == areaId && x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo {itemId} no existe en el area {areaId}.");
            _context.ItemsAreas.Remove(entity);
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
        public async Task<IEnumerable<ItemsArea>> GetAsync(short areaId, CancellationToken ct = default)
        {
            return await _context.ItemsAreas.AsNoTracking()
                .Include(i => i.Area)
                .Include(i => i.Item.Line)
                .Include(i => i.Item.Currency)
                .Include(i => i.Item.CifMeasureUnit)
                .Include(i => i.Item.FobMeasureUnit)
                .Where(w => w.AreaId == areaId).ToListAsync(ct);
        }
    }
}
