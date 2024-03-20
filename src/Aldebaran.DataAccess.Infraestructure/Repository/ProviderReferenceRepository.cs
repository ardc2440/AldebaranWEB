using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ProviderReferenceRepository : IProviderReferenceRepository
    {
        private readonly AldebaranDbContext _context;
        public ProviderReferenceRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ProviderReference providerReference, CancellationToken ct = default)
        {
            await _context.ProviderReferences.AddAsync(providerReference, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int providerId, int referenceId, CancellationToken ct = default)
        {
            var entity = await _context.ProviderReferences.FirstOrDefaultAsync(x => x.ProviderId == providerId && x.ReferenceId == referenceId, ct) ?? throw new KeyNotFoundException($"Referencia con id {referenceId} no existe para el proveedor con id {providerId}.");
            _context.ProviderReferences.Remove(entity);
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

        public async Task<ProviderReference?> FindAsync(int providerId, int referenceId, CancellationToken ct = default)
        {
            return await _context.ProviderReferences.AsNoTracking()
                .Include(i => i.ItemReference.Item.Currency)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.ItemReference.Item.CifMeasureUnit)
                .Include(i => i.ItemReference.Item.FobMeasureUnit)
                .Include(i => i.Provider.City.Department.Country)
                .Include(i => i.Provider.IdentityType)
                .FirstOrDefaultAsync(w => w.ProviderId == providerId && w.ReferenceId == referenceId, ct);
        }

        public async Task<IEnumerable<ProviderReference>> GetByProviderIdAsync(int providerId, CancellationToken ct = default)
        {
            return await _context.ProviderReferences.AsNoTracking()
                .Where(w => w.ProviderId == providerId)
                .Include(i => i.ItemReference.Item.Currency)
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.ItemReference.Item.CifMeasureUnit)
                .Include(i => i.ItemReference.Item.FobMeasureUnit)
                .Include(i => i.Provider.City.Department.Country)
                .Include(i => i.Provider.IdentityType)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ProviderReference>> GetProviderReferecesReport(CancellationToken ct = default)
        {
            return await _context.ProviderReferences.AsNoTracking()
                .Include(i => i.ItemReference.Item.Line)
                .Include(i => i.Provider.City.Department.Country)
                .Include(i => i.Provider.IdentityType)
                .ToListAsync(ct);
        }

    }

}
