using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly AldebaranDbContext _context;
        public ProviderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsByIdentificationNumber(string identificationNumber, CancellationToken ct = default)
        {
            return await _context.Providers.AsNoTracking().AnyAsync(i => i.IdentityNumber.Trim().ToLower() == identificationNumber.Trim().ToLower(), ct);
        }
        public async Task<bool> ExistsByCode(string code, CancellationToken ct = default)
        {
            return await _context.Providers.AsNoTracking().AnyAsync(i => i.ProviderCode.Trim().ToLower() == code.Trim().ToLower(), ct);
        }
        public async Task<bool> ExistsByName(string name, CancellationToken ct = default)
        {
            return await _context.Providers.AsNoTracking().AnyAsync(i => i.ProviderName.Trim().ToLower() == name.Trim().ToLower(), ct);
        }

        public async Task AddAsync(Provider provider, CancellationToken ct = default)
        {
            await _context.Providers.AddAsync(provider, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int providerId, CancellationToken ct = default)
        {
            var entity = await _context.Providers.FirstOrDefaultAsync(x => x.ProviderId == providerId, ct) ?? throw new KeyNotFoundException($"Proveedor con id {providerId} no existe.");
            _context.Providers.Remove(entity);
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

        public async Task<Provider?> FindAsync(int providerId, CancellationToken ct = default)
        {
            return await _context.Providers.AsNoTracking()
               .Include(i => i.City.Department.Country)
               .Include(i => i.IdentityType)
               .FirstOrDefaultAsync(w => w.ProviderId == providerId, ct);
        }

        public async Task<IEnumerable<Provider>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Providers.AsNoTracking()
               .Include(i => i.City.Department.Country)
               .Include(i => i.IdentityType)
               .ToListAsync(ct);
        }

        public async Task<IEnumerable<Provider>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Providers.AsNoTracking()
              .Where(w => w.IdentityNumber.Contains(searchKey) || w.ProviderCode.Contains(searchKey) || w.ProviderName.Contains(searchKey) || w.ProviderAddress.Contains(searchKey) || w.Phone.Contains(searchKey) || w.Fax.Contains(searchKey) || w.Email.Contains(searchKey) || w.ContactPerson.Contains(searchKey))
              .Include(i => i.City.Department.Country)
              .Include(i => i.IdentityType)
              .ToListAsync(ct);
        }

        public async Task UpdateAsync(int providerId, Provider provider, CancellationToken ct = default)
        {
            var entity = await _context.Providers.FirstOrDefaultAsync(x => x.ProviderId == providerId, ct) ?? throw new KeyNotFoundException($"Proveedor con id {providerId} no existe.");
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
            await _context.SaveChangesAsync(ct);
        }
    }
}
