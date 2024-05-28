using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderReferenceRepository : RepositoryBase<AldebaranDbContext>, IProviderReferenceRepository
    {
        public ProviderReferenceRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddRangeAsync(List<ProviderReference> providerReferences, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.ProviderReferences.AddRangeAsync(providerReferences, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task AddAsync(ProviderReference providerReference, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.ProviderReferences.AddAsync(providerReference, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task DeleteAsync(int providerId, int referenceId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.ProviderReferences.FirstOrDefaultAsync(x => x.ProviderId == providerId && x.ReferenceId == referenceId, ct) ?? throw new KeyNotFoundException($"Referencia con id {referenceId} no existe para el proveedor con id {providerId}.");
                dbContext.ProviderReferences.Remove(entity);
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

        public async Task<ProviderReference?> FindAsync(int providerId, int referenceId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ProviderReferences.AsNoTracking()
               .Include(i => i.ItemReference.Item.Currency)
               .Include(i => i.ItemReference.Item.Line)
               .Include(i => i.ItemReference.Item.CifMeasureUnit)
               .Include(i => i.ItemReference.Item.FobMeasureUnit)
               .Include(i => i.Provider.City.Department.Country)
               .Include(i => i.Provider.IdentityType)
               .FirstOrDefaultAsync(w => w.ProviderId == providerId && w.ReferenceId == referenceId, ct);
            }, ct);
        }

        public async Task<IEnumerable<ProviderReference>> GetByProviderIdAsync(int providerId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ProviderReferences.AsNoTracking()
                            .Where(w => w.ProviderId == providerId)
                            .Include(i => i.ItemReference.Item.Currency)
                            .Include(i => i.ItemReference.Item.Line)
                            .Include(i => i.ItemReference.Item.CifMeasureUnit)
                            .Include(i => i.ItemReference.Item.FobMeasureUnit)
                            .Include(i => i.Provider.City.Department.Country)
                            .Include(i => i.Provider.IdentityType)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<ProviderReference>> GetProviderReferecesReport(CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ProviderReferences.AsNoTracking()
                            .Include(i => i.ItemReference.Item.Line)
                            .Include(i => i.Provider.City.Department.Country)
                            .Include(i => i.Provider.IdentityType)
                            .Include(i => i.ItemReference.ReferencesWarehouses)
                            .ToListAsync(ct);
            }, ct);
        }
    }
}
