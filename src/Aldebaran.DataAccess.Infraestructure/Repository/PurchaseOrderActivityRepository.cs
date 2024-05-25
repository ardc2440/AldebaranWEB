using Aldebaran.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderActivityRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderActivityRepository
    {
        public PurchaseOrderActivityRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.PurchaseOrderActivities.AddAsync(purchaseOrderActivity, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(purchaseOrderActivity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task DeleteAsync(int purchaseOrderActivityId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrderActivities.FirstOrDefaultAsync(x => x.PurchaseOrderActivityId == purchaseOrderActivityId, ct) ?? throw new KeyNotFoundException($"Actividad de la orden de compra con id {purchaseOrderActivityId} no existe.");
                dbContext.PurchaseOrderActivities.Remove(entity);
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task<PurchaseOrderActivity?> FindAsync(int purchaseOrderActivityId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderActivities.AsNoTracking()
                           .Include(p => p.PurchaseOrder)
                           .Include(p => p.ActivityEmployee.Area)
                           .Include(p => p.ActivityEmployee.IdentityType)
                           .Include(p => p.Employee.Area)
                           .Include(p => p.Employee.IdentityType)
                           .FirstOrDefaultAsync(p => p.PurchaseOrderActivityId == purchaseOrderActivityId, ct);
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderActivities.AsNoTracking()
                           .Include(p => p.PurchaseOrder)
                           .Include(p => p.ActivityEmployee.Area)
                           .Include(p => p.ActivityEmployee.IdentityType)
                           .Include(p => p.Employee.Area)
                           .Include(p => p.Employee.IdentityType)
                           .Where(p => p.PurchaseOrderId == purchaseOrderId)
                           .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int purchaseOrderActivityId, PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrderActivities.FirstOrDefaultAsync(x => x.PurchaseOrderActivityId == purchaseOrderActivityId, ct) ?? throw new KeyNotFoundException($"Actividad de la orden de compra con id {purchaseOrderActivityId} no existe.");
                entity.ExecutionDate = purchaseOrderActivity.ExecutionDate;
                entity.ActivityDescription = purchaseOrderActivity.ActivityDescription;
                entity.EmployeeId = purchaseOrderActivity.EmployeeId;
                entity.ActivityEmployeeId = purchaseOrderActivity.ActivityEmployeeId;
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }
    }
}
