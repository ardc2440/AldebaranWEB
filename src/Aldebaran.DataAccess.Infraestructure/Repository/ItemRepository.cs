using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemRepository : RepositoryBase<AldebaranDbContext>, IItemRepository
    {
        public ItemRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(Item item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.Items.AddAsync(item, ct);
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }

        public async Task DeleteAsync(int itemId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Items.FirstOrDefaultAsync(x => x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo {itemId} no existe.");
                dbContext.Items.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task<Item?> FindAsync(int itemId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Items.AsNoTracking()
                           .Include(i => i.Currency)
                           .Include(i => i.Line)
                           .Include(i => i.CifMeasureUnit)
                           .Include(i => i.FobMeasureUnit)
                           .FirstOrDefaultAsync(f => f.ItemId == itemId, ct);
            }, ct);
        }

        public async Task<bool> ExistsByIternalReference(string internalReference, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Items.AsNoTracking().AnyAsync(i => i.InternalReference.Trim().ToLower() == internalReference.Trim().ToLower(), ct);
            }, ct);
        }

        public async Task<bool> ExistsByItemName(string itemName, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Items.AsNoTracking().AnyAsync(i => i.ItemName.Trim().ToLower() == itemName.Trim().ToLower(), ct);
            }, ct);
        }

        public async Task<IEnumerable<Item>> GetAsync(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Items.AsNoTracking()
                          .Include(i => i.Currency)
                          .Include(i => i.Line)
                          .Include(i => i.CifMeasureUnit)
                          .Include(i => i.FobMeasureUnit)
                          .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<Item>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Items.AsNoTracking()
               .Include(i => i.Currency)
               .Include(i => i.Line)
               .Include(i => i.CifMeasureUnit)
               .Include(i => i.FobMeasureUnit)
               .Where(w => w.InternalReference.Contains(searchKey) ||
                           w.ItemName.Contains(searchKey) ||
                           w.ProviderReference.Contains(searchKey) ||
                           w.ProviderItemName.Contains(searchKey) ||
                           w.Notes.Contains(searchKey) ||
                           w.Line.LineName.Equals(searchKey) ||
                           w.Line.LineCode.Equals(searchKey) ||
                           w.Currency.CurrencyName.Equals(searchKey))
               .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int itemId, Item item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Items.FirstOrDefaultAsync(x => x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo con id {itemId} no existe.");
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
                await dbContext.SaveChangesAsync(ct);
                return Task.CompletedTask;
            }, ct);
        }
    }
}
