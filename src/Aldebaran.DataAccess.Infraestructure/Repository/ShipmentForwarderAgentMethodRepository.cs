using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class ShipmentForwarderAgentMethodRepository : RepositoryBase<AldebaranDbContext>, IShipmentForwarderAgentMethodRepository
    {
        public ShipmentForwarderAgentMethodRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                await dbContext.ShipmentForwarderAgentMethods.AddAsync(shipmentForwarderAgentMethod, ct);
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }

        public async Task DeleteAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.ShipmentForwarderAgentMethods.FirstOrDefaultAsync(x => x.ShipmentForwarderAgentMethodId == shipmentForwarderAgentMethodId, ct) ?? throw new KeyNotFoundException($"Método de envío con id {shipmentForwarderAgentMethodId} no existe.");
                dbContext.ShipmentForwarderAgentMethods.Remove(entity);
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

        public async Task<ShipmentForwarderAgentMethod?> FindAsync(int shipmentForwarderAgentMethodId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ShipmentForwarderAgentMethods.AsNoTracking()
                   .Include(i => i.ShipmentMethod)
                   .Include(i => i.ForwarderAgent)
                  .FirstOrDefaultAsync(w => w.ShipmentForwarderAgentMethodId == shipmentForwarderAgentMethodId, ct);
            }, ct);
        }

        public async Task<IEnumerable<ShipmentForwarderAgentMethod>> GetByForwarderAgentIdAsync(int forwarderAgentId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.ShipmentForwarderAgentMethods.AsNoTracking()
                   .Include(i => i.ShipmentMethod)
                   .Include(i => i.ForwarderAgent)
                   .Where(w => w.ForwarderAgentId == forwarderAgentId)
                   .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int shipmentForwarderAgentMethodId, ShipmentForwarderAgentMethod shipmentForwarderAgentMethod, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.ShipmentForwarderAgentMethods.FirstOrDefaultAsync(x => x.ShipmentForwarderAgentMethodId == shipmentForwarderAgentMethodId, ct) ?? throw new KeyNotFoundException($"Método de envío con id {shipmentForwarderAgentMethodId} no existe.");
                entity.ShipmentMethodId = shipmentForwarderAgentMethod.ShipmentMethodId;
                entity.ForwarderAgentId = shipmentForwarderAgentMethod.ForwarderAgentId;
                await dbContext.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
