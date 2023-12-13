using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task DeleteAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId, ct) ?? throw new KeyNotFoundException($"Orden con id {purchaseOrderId} no existe.");
            _context.PurchaseOrders.Remove(entity);
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

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
                .Include(i => i.Employee.Area)
                .Include(i => i.Employee.IdentityType)
                .Include(i => i.ForwarderAgent.Forwarder)
                .Include(i => i.Provider.IdentityType)
                .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
                .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
                .Include(i => i.StatusDocumentType.DocumentType)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders.AsNoTracking()
               .Include(i => i.Employee.Area)
               .Include(i => i.Employee.IdentityType)
               .Include(i => i.ForwarderAgent.Forwarder)
               .Include(i => i.Provider.IdentityType)
               .Include(i => i.ShipmentForwarderAgentMethod.ShipmentMethod)
               .Include(i => i.ShipmentForwarderAgentMethod.ForwarderAgent)
               .Include(i => i.StatusDocumentType.DocumentType)
               .Where(w => w.OrderNumber.Contains(searchKey) || w.ImportNumber.Contains(searchKey) || w.EmbarkationPort.Contains(searchKey) || w.ProformaNumber.Contains(searchKey))
               .ToListAsync(ct);
        }
    }
}
