using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderRepository : RepositoryBase<AldebaranDbContext>, IProviderRepository
    {
        public ProviderRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Providers.AsNoTracking().AnyAsync(i => i.IdentityNumber.Trim().ToLower() == identificationNumber.Trim().ToLower(), ct);

            }, ct);
        }

        public async Task<bool> ExistsByCode(string code, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Providers.AsNoTracking().AnyAsync(i => i.ProviderCode.Trim().ToLower() == code.Trim().ToLower(), ct);

            }, ct);
        }

        public async Task<bool> ExistsByName(string name, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Providers.AsNoTracking().AnyAsync(i => i.ProviderName.Trim().ToLower() == name.Trim().ToLower(), ct);

            }, ct);
        }

        public async Task AddAsync(Provider provider, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.Providers.AddAsync(provider, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task DeleteAsync(int providerId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Providers.FirstOrDefaultAsync(x => x.ProviderId == providerId, ct) ?? throw new KeyNotFoundException($"Proveedor con id {providerId} no existe.");
                dbContext.Providers.Remove(entity);
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

        public async Task<Provider?> FindAsync(int providerId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.Providers.AsNoTracking()
              .Include(i => i.City.Department.Country)
              .Include(i => i.IdentityType)
              .FirstOrDefaultAsync(w => w.ProviderId == providerId, ct);
            }, ct);
        }

        public async Task<(IEnumerable<Provider>, int)> GetAsync(int? skip=null, int? top = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.Providers.AsNoTracking()
                  .Include(i => i.City.Department.Country)
                  .Include(i => i.IdentityType)
                  .OrderBy(o => o.ProviderName);

              if (skip != null && top != null)
                    return (await a.Skip(skip.Value).Take(top.Value).ToListAsync(), await a.CountAsync(ct));

              return (await a.ToListAsync(), await a.CountAsync(ct));
            }, ct);
        }

        public async Task<(IEnumerable<Provider>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var a = dbContext.Providers.AsNoTracking()
                          .Where(w => w.IdentityNumber.Contains(searchKey) || w.ProviderCode.Contains(searchKey) || w.ProviderName.Contains(searchKey) || w.ProviderAddress.Contains(searchKey) || w.Phone.Contains(searchKey) || w.Fax.Contains(searchKey) || w.Email.Contains(searchKey) || w.ContactPerson.Contains(searchKey))
                          .Include(i => i.City.Department.Country)
                          .Include(i => i.IdentityType)
                          .OrderBy(o => o.ProviderName);

                return (await a.Skip(skip).Take(top).ToListAsync(), await a.CountAsync(ct));

            }, ct);
        }

        public async Task UpdateAsync(int providerId, Provider provider, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.Providers.FirstOrDefaultAsync(x => x.ProviderId == providerId, ct) ?? throw new KeyNotFoundException($"Proveedor con id {providerId} no existe.");
                entity.IdentityTypeId = provider.IdentityTypeId;
                entity.IdentityNumber = provider.IdentityNumber;
                entity.ProviderCode = provider.ProviderCode;
                entity.ProviderName = provider.ProviderName;
                entity.ProviderAddress = provider.ProviderAddress;
                entity.Phone = provider.Phone;
                entity.Fax = provider.Fax;
                entity.Email = provider.Email;
                entity.ContactPerson = provider.ContactPerson;
                entity.CityId = provider.CityId;
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
