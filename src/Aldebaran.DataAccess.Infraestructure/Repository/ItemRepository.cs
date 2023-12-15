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

        public async Task AddAsync(Item item, CancellationToken ct = default)
        {
            await _context.Items.AddAsync(item, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int itemId, CancellationToken ct = default)
        {
            var entity = await _context.Items.FirstOrDefaultAsync(x => x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo {itemId} no existe.");
            _context.Items.Remove(entity);
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

        public async Task<IEnumerable<Item>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Items.AsNoTracking()
                .Where(w => w.InternalReference.Contains(searchKey) || w.ItemName.Contains(searchKey) || w.ProviderReference.Contains(searchKey) || w.ProviderItemName.Contains(searchKey) || w.Notes.Contains(searchKey))
                .Include(i => i.Currency)
                .Include(i => i.Line)
                .Include(i => i.CifMeasureUnit)
                .Include(i => i.FobMeasureUnit)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int itemId, Item item, CancellationToken ct = default)
        {
            var entity = await _context.Items.FirstOrDefaultAsync(x => x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo con id {itemId} no existe.");
            entity.InternalReference = item.InternalReference;
            entity.ItemName = item.ItemName;
            entity.ProviderReference = item.ProviderReference;
            entity.ProviderItemName = item.ProviderItemName;
            entity.FobCost = item.FobCost;
            entity.CurrencyId = item.CurrencyId;
            entity.Notes = item.Notes;
            entity.IsExternalInventory = item.IsExternalInventory;
            entity.CifCost = item.CifCost;
            entity.Volume = item.Volume;
            entity.Weight = item.Weight;
            entity.FobMeasureUnitId = item.FobMeasureUnitId;
            entity.CifMeasureUnitId = item.CifMeasureUnitId;
            entity.IsDomesticProduct = item.IsDomesticProduct;
            entity.IsActive = item.IsActive;
            entity.IsCatalogVisible = item.IsCatalogVisible;
            await _context.SaveChangesAsync(ct);
        }
    }
}
