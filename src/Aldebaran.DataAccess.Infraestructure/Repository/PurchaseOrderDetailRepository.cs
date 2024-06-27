using Aldebaran.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderDetailRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderDetailRepository
    {
        public PurchaseOrderDetailRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task AddAsync(PurchaseOrderDetail purchaseOrder, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.PurchaseOrderDetails.AddAsync(purchaseOrder, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(purchaseOrder).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task DeleteAsync(int purchaseOrderDetailId, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrderDetails.FirstOrDefaultAsync(x => x.PurchaseOrderDetailId == purchaseOrderDetailId, ct) ?? throw new KeyNotFoundException($"Detalle de la orden de compra con id {purchaseOrderDetailId} no existe.");
                dbContext.PurchaseOrderDetails.Remove(entity);
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

        public async Task<PurchaseOrderDetail?> FindAsync(int purchaseOrderDetailId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderDetails.AsNoTracking()
                            .Include(p => p.PurchaseOrder)
                            .Include(p => p.ItemReference.Item.Line)
                            .Include(p => p.Warehouse)
                            .FirstOrDefaultAsync(p => p.PurchaseOrderDetailId == purchaseOrderDetailId, ct);
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderDetails.AsNoTracking()
                            .Include(p => p.PurchaseOrder)
                            .Include(p => p.ItemReference.Item.Line)
                            .Include(p => p.Warehouse)
                            .Where(p => p.PurchaseOrderId == purchaseOrderId)
                            .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderDetails.AsNoTracking()
               .Include(p => p.PurchaseOrder)
               .Include(p => p.ItemReference.Item.Line)
               .Include(p => p.Warehouse)
               .Where(p => (p.ReferenceId == referenceId || !referenceId.HasValue) && p.PurchaseOrder.StatusDocumentTypeId == statusOrder)
               .ToListAsync(ct);
            }, ct);
        }

        public async Task UpdateAsync(int purchaseOrderDetailId, PurchaseOrderDetail purchaseOrder, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrderDetails.FirstOrDefaultAsync(x => x.PurchaseOrderDetailId == purchaseOrderDetailId, ct) ?? throw new KeyNotFoundException($"Detalle de la orden de compra con id {purchaseOrderDetailId} no existe.");
                entity.WarehouseId = purchaseOrder.WarehouseId;
                entity.ReceivedQuantity = purchaseOrder.ReceivedQuantity;
                entity.RequestedQuantity = purchaseOrder.RequestedQuantity;
                try
                {
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(entity).State = EntityState.Unchanged;
                    throw;
                }
            }, ct);
        }

        public async Task<bool> IsValidPurchaseOrderVariation(int providerId, int referenceId, int? purchaseOrderId = null, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                var IsValidVariation = new SqlParameter();

                IsValidVariation.ParameterName = "@IS_VALID_VARIATION";
                IsValidVariation.SqlDbType = SqlDbType.Bit;
                    IsValidVariation.Direction = ParameterDirection.Output;
                
                try
                {
                    await dbContext.Database
                        .ExecuteSqlRawAsync("EXEC SP_IS_VALID_PURCHASE_ORDER_VARIATION @PROVIDER_ID={0}, @REFERENCE_ID={1}, @PURCHASE_ORDER_ID={2}, @IS_VALID_VARIATION = {3} OUT",
                        providerId, referenceId, purchaseOrderId.HasValue ? purchaseOrderId : -1, IsValidVariation);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                bool result = (bool)IsValidVariation.Value;

                return await Task.FromResult(result);
            }, ct);
        }
    }
}
