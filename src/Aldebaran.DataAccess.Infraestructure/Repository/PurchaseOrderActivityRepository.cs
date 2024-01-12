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

        public async Task AddAsync(PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default)
        {
            try
            {
                await _context.PurchaseOrderActivities.AddAsync(purchaseOrderActivity, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(purchaseOrderActivity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task DeleteAsync(int purchaseOrderActivityId, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrderActivities.FirstOrDefaultAsync(x => x.PurchaseOrderActivityId == purchaseOrderActivityId, ct) ?? throw new KeyNotFoundException($"Actividad de la orden de compra con id {purchaseOrderActivityId} no existe.");
            _context.PurchaseOrderActivities.Remove(entity);
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

        public async Task<PurchaseOrderActivity?> FindAsync(int purchaseOrderActivityId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderActivities.AsNoTracking()
               .Include(p => p.PurchaseOrder)
               .Include(p => p.ActivityEmployee.Area)
               .Include(p => p.ActivityEmployee.IdentityType)
               .Include(p => p.Employee.Area)
               .Include(p => p.Employee.IdentityType)
               .FirstOrDefaultAsync(p => p.PurchaseOrderActivityId == purchaseOrderActivityId, ct);
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

        public async Task UpdateAsync(int purchaseOrderActivityId, PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrderActivities.FirstOrDefaultAsync(x => x.PurchaseOrderActivityId == purchaseOrderActivityId, ct) ?? throw new KeyNotFoundException($"Actividad de la orden de compra con id {purchaseOrderActivityId} no existe.");
            entity.ExecutionDate = purchaseOrderActivity.ExecutionDate;
            entity.ActivityDescription = purchaseOrderActivity.ActivityDescription;
            entity.EmployeeId = purchaseOrderActivity.EmployeeId;
            entity.ActivityEmployeeId = purchaseOrderActivity.ActivityEmployeeId;
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
    }

}
