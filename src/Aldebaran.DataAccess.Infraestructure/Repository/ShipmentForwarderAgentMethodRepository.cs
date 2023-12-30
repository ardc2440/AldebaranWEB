using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShipmentForwarderAgentMethodRepository : IShipmentForwarderAgentMethodRepository
    {
        private readonly AldebaranDbContext _context;
        public ShipmentForwarderAgentMethodRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default)
        {
            await _context.ShipmentForwarderAgentMethods.AddAsync(shipmentForwarderAgentMethod, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default)
        {
            var entity = await _context.ShipmentForwarderAgentMethods.FirstOrDefaultAsync(x => x.ShipmentForwarderAgentMethodId == shipmentForwarderAgentMethodId, ct) ?? throw new KeyNotFoundException($"Método de envío con id {shipmentForwarderAgentMethodId} no existe.");
            _context.ShipmentForwarderAgentMethods.Remove(entity);
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

        public async Task<ShipmentForwarderAgentMethod?> FindAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default)
        {
            return await _context.ShipmentForwarderAgentMethods.AsNoTracking()
                .Include(i => i.ShipmentMethod)
                .Include(i => i.ForwarderAgent)
               .FirstOrDefaultAsync(w => w.ShipmentForwarderAgentMethodId == shipmentForwarderAgentMethodId, ct);
        }

        public async Task<IEnumerable<ShipmentForwarderAgentMethod>> GetByForwarderAgentIdAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            return await _context.ShipmentForwarderAgentMethods.AsNoTracking()
                .Include(i => i.ShipmentMethod)
                .Include(i => i.ForwarderAgent)
                .Where(w => w.ForwarderAgentId == forwarderAgentId)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int shipmentForwarderAgentMethodId, ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default)
        {
            var entity = await _context.ShipmentForwarderAgentMethods.FirstOrDefaultAsync(x => x.ShipmentForwarderAgentMethodId == shipmentForwarderAgentMethodId, ct) ?? throw new KeyNotFoundException($"Método de envío con id {shipmentForwarderAgentMethodId} no existe.");
            entity.ShipmentMethodId = shipmentForwarderAgentMethod.ShipmentMethodId;
            entity.ForwarderAgentId = shipmentForwarderAgentMethod.ForwarderAgentId;
            await _context.SaveChangesAsync(ct);
        }
    }
}
