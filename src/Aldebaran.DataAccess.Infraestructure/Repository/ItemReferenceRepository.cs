using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemReferenceRepository : IItemReferenceRepository
    {
        private readonly AldebaranDbContext _context;
        public ItemReferenceRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ItemReference itemReference, CancellationToken ct = default)
        {
            await _context.ItemReferences.AddAsync(itemReference, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int itemReferenceId, CancellationToken ct = default)
        {
            var entity = await _context.ItemReferences.FirstOrDefaultAsync(x => x.ReferenceId == itemReferenceId, ct) ?? throw new KeyNotFoundException($"Referencia con id {itemReferenceId} no existe.");
            _context.ItemReferences.Remove(entity);
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

        public async Task<ItemReference?> FindAsync(int itemReferenceId, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Include(i => i.Item.Currency)
                .Include(i => i.Item.CifMeasureUnit)
                .Include(i => i.Item.FobMeasureUnit)
                .FirstOrDefaultAsync(w => w.ReferenceId == itemReferenceId, ct);
        }

        public async Task<bool> ExistsByReferenceCode(string referenceCode, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking().AnyAsync(w => w.ReferenceCode.Trim().ToLower() == referenceCode.Trim().ToLower());
        }
        public async Task<bool> ExistsByReferenceName(string referenceName, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking().AnyAsync(w => w.ReferenceName.Trim().ToLower() == referenceName.Trim().ToLower());
        }
        public async Task<IEnumerable<ItemReference>> GetByStatusAsync(bool isActive, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
               .Where(w => w.IsActive == isActive && w.Item.IsActive == isActive && w.Item.Line.IsActive == isActive)
               .Include(i => i.Item.Line)
               .Include(i => i.Item.Currency)
               .Include(i => i.Item.CifMeasureUnit)
               .Include(i => i.Item.FobMeasureUnit)
               .ToListAsync(ct);
        }

        public async Task<IEnumerable<ItemReference>> GetByItemIdAsync(int itemId, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
               .Where(w => w.ItemId == itemId)
               .Include(i => i.Item.Line)
               .Include(i => i.Item.Currency)
               .Include(i => i.Item.CifMeasureUnit)
               .Include(i => i.Item.FobMeasureUnit)
               .ToListAsync(ct);
        }

        public async Task UpdateAsync(int itemReferenceId, ItemReference itemReference, CancellationToken ct = default)
        {
            var entity = await _context.ItemReferences.FirstOrDefaultAsync(x => x.ReferenceId == itemReferenceId, ct) ?? throw new KeyNotFoundException($"Referencia con id {itemReferenceId} no existe.");
            entity.ReferenceCode = itemReference.ReferenceCode;
            entity.ProviderReferenceCode = itemReference.ProviderReferenceCode;
            entity.ReferenceName = itemReference.ReferenceName;
            entity.ProviderReferenceName = itemReference.ProviderReferenceName;
            entity.Notes = itemReference.Notes;
            entity.InventoryQuantity = itemReference.InventoryQuantity;
            entity.OrderedQuantity = itemReference.OrderedQuantity;
            entity.ReservedQuantity = itemReference.ReservedQuantity;
            entity.WorkInProcessQuantity = itemReference.WorkInProcessQuantity;
            entity.IsActive = itemReference.IsActive;
            entity.IsSoldOut = itemReference.IsSoldOut;
            entity.AlarmMinimumQuantity = itemReference.AlarmMinimumQuantity;
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<ItemReference>> GetAsync(CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
               .Include(i => i.Item.Line)
               .Include(i => i.Item.Currency)
               .Include(i => i.Item.CifMeasureUnit)
               .Include(i => i.Item.FobMeasureUnit)
               .ToListAsync(ct);
        }

        public async Task<List<ItemReference>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Include(i => i.Item.Currency)
                .Include(i => i.Item.CifMeasureUnit)
                .Include(i => i.Item.FobMeasureUnit)
                .Where(i => i.Item.ItemName.Contains(searchKey) ||
                          i.Item.Line.LineName.Contains(searchKey) ||
                          i.Item.Line.LineCode.Contains(searchKey) ||
                          i.Item.InternalReference.Contains(searchKey) ||
                          i.Item.Notes.Contains(searchKey) ||
                          i.ReferenceCode.Contains(searchKey) ||
                          i.ReferenceName.Contains(searchKey))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(i => i.AlarmMinimumQuantity > 0 && i.InventoryQuantity <= i.AlarmMinimumQuantity && i.IsActive && i.Item.IsActive)
                .ToListAsync(ct);
        }
        public List<ItemReference> GetAllReferencesWithMinimumQuantity()
        {
            return _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(i => i.AlarmMinimumQuantity > 0 && i.InventoryQuantity <= i.AlarmMinimumQuantity && i.IsActive && i.Item.IsActive)
                .ToList();
        }
        public async Task<IEnumerable<ItemReference>> GetAllReferencesOutOfStockAsync(CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(i => i.InventoryQuantity <= 0 && i.AlarmMinimumQuantity == 0 && i.IsActive && i.Item.IsActive)
                .ToListAsync(ct);
        }

        public List<ItemReference> GetAllReferencesOutOfStock()
        {
            return _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(i => i.InventoryQuantity <= 0 && i.AlarmMinimumQuantity == 0 && i.IsActive && i.Item.IsActive)
                .ToList();
        }

        public async Task<IEnumerable<ItemReference>> GetReportsReferencesAsync(bool? isReferenceActive = null, bool? isItemActive = null, bool? isExternalInventory = null, CancellationToken ct = default)
        {
            return await _context.ItemReferences.AsNoTracking()
                .Include(i => i.Item.Line)
                .Where(i => (i.IsActive || isReferenceActive == null) && (i.Item.IsActive || isItemActive == null) && (i.Item.IsExternalInventory || isExternalInventory == null))
                .ToListAsync(ct);
        }
    }
}
