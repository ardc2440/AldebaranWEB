using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ForwarderRepository : IForwarderRepository
    {
        private readonly AldebaranDbContext _context;
        public ForwarderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(Forwarder forwarder, CancellationToken ct = default)
        {
            await _context.Forwarders.AddAsync(forwarder, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int forwarderId, CancellationToken ct = default)
        {
            var entity = await _context.Forwarders.FirstOrDefaultAsync(x => x.ForwarderId == forwarderId, ct) ?? throw new KeyNotFoundException($"Transportadora con id {forwarderId} no existe.");
            _context.Forwarders.Remove(entity);
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

        public async Task<Forwarder?> FindAsync(int forwarderId, CancellationToken ct = default)
        {
            return await _context.Forwarders.AsNoTracking()
                .Include(i => i.City.Department.Country)
                .FirstOrDefaultAsync(w => w.ForwarderId == forwarderId, ct);
        }

        public async Task<IEnumerable<Forwarder>> GetAsync(CancellationToken ct = default)
        {
            return await _context.Forwarders.AsNoTracking()
                .Include(i => i.City.Department.Country)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Forwarder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.Forwarders.AsNoTracking().Where(w => w.ForwarderName.Contains(searchKey) || w.Phone1.Contains(searchKey) || w.Phone2.Contains(searchKey) || w.Fax.Contains(searchKey) || w.ForwarderAddress.Contains(searchKey) || w.Mail1.Contains(searchKey) || w.Mail2.Contains(searchKey))
                .Include(i => i.City.Department.Country)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int forwarderId, Forwarder forwarder, CancellationToken ct = default)
        {
            var entity = await _context.Forwarders.FirstOrDefaultAsync(x => x.ForwarderId == forwarderId, ct) ?? throw new KeyNotFoundException($"Transportadora con id {forwarderId} no existe.");
            entity.ForwarderName = forwarder.ForwarderName;
            entity.Phone1 = forwarder.Phone1;
            entity.Phone2 = forwarder.Phone2;
            entity.Fax = forwarder.Fax;
            entity.ForwarderAddress = forwarder.ForwarderAddress;
            entity.Mail1 = forwarder.Mail1;
            entity.Mail2 = forwarder.Mail2;
            entity.CityId = forwarder.CityId;
            await _context.SaveChangesAsync(ct);
        }
    }
}
