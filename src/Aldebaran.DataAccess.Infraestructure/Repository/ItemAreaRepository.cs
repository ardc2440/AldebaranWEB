using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ItemAreaRepository : RepositoryBase<AldebaranDbContext>, IItemAreaRepository
    {
        public ItemAreaRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(ItemsArea item, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.ItemsAreas.AddAsync(item, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task DeleteAsync(short areaId, int itemId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.ItemsAreas.FirstOrDefaultAsync(x => x.AreaId == areaId && x.ItemId == itemId, ct) ?? throw new KeyNotFoundException($"Artículo {itemId} no existe en el area {areaId}.");
                dbContext.ItemsAreas.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<IEnumerable<ItemsArea>> GetByAreaIdAsync(short areaId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ItemsAreas.AsNoTracking()
                            .Include(i => i.Area)
                            .Include(i => i.Item.Line)
                            .Include(i => i.Item.Currency)
                            .Include(i => i.Item.CifMeasureUnit)
                            .Include(i => i.Item.FobMeasureUnit)
                            .Where(w => w.AreaId == areaId).ToListAsync(ct);
            }, ct);
        }
    }
}
