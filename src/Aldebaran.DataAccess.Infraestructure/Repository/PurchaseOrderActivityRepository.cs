using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderActivityRepository : IPurchaseOrderActivityRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderActivityRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderActivities.AsNoTracking()
               .Include(p => p.PurchaseOrder)
               .Include(p => p.ActivityEmployee.Area)
               .Include(p => p.ActivityEmployee.IdentityType)
               .Include(p => p.Employee.Area)
               .Include(p => p.Employee.IdentityType)
               .Where(p => p.PurchaseOrderId.Equals(purchaseOrderId))
               .ToListAsync(ct);
        }
    }

}
