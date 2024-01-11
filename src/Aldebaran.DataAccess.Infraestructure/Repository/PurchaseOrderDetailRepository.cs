using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderDetailRepository : IPurchaseOrderDetailRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderDetailRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderDetails.AsNoTracking()
                .Include(p => p.PurchaseOrder)
                .Include(p => p.ItemReference.Item.Line)
                .Include(p => p.Warehouse)
                .Where(p => p.PurchaseOrderId.Equals(purchaseOrderId))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int referenceId, int statusOrder, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderDetails.AsNoTracking()
                .Include(p => p.PurchaseOrder)
                .Include(p => p.ItemReference.Item.Line)
                .Include(p => p.Warehouse)
                .Where(p => p.ReferenceId.Equals(referenceId) && p.PurchaseOrder.StatusDocumentTypeId.Equals(statusOrder))
                .ToListAsync(ct);
        }
    }
}
