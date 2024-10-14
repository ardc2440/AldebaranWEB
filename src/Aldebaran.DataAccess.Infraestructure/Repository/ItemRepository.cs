using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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
            }, ct);
        }

        public async Task DeleteAsync(int itemId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Items.FirstOrDefaultAsync(x => x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo {itemId} no existe.");
                var entityPackaging = await dbContext.Packagings.FirstOrDefaultAsync(x => x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo {itemId} no existe.");
                dbContext.Packagings.Remove(entityPackaging);
                dbContext.Items.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    dbContext.Entry(entityPackaging).State = EntityState.Unchanged;
                    throw;
                }
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

        public async Task<(IEnumerable<Item>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.Items.AsNoTracking()
                            .Include(i => i.Currency)
                            .Include(i => i.Line)
                            .Include(i => i.CifMeasureUnit)
                            .Include(i => i.FobMeasureUnit);

                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<Item>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.Items.AsNoTracking()
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
                                       w.Currency.CurrencyName.Equals(searchKey));
                return (await a.Skip(skip).Take(top).ToListAsync(ct), await a.CountAsync(ct));
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
                entity.Packagings = item.Packagings;
                entity.IsSpecialImport = item.IsSpecialImport;
                entity.IsSaleOff = item.IsSaleOff;
                entity.ApplyPreorder = item.ApplyPreorder;


                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task<(IEnumerable<Item> Items, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var query = dbContext.Items.AsNoTracking()
                          .Include(i => i.Currency)
                          .Include(i => i.Line)
                          .Include(i => i.CifMeasureUnit)
                          .Include(i => i.FobMeasureUnit)
                   .AsQueryable();
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }
                var count = query.Count();
                var data = await query.Skip(skip).Take(take).ToListAsync(ct);
                return (data, count);
            }, ct);
        }
    }
}
