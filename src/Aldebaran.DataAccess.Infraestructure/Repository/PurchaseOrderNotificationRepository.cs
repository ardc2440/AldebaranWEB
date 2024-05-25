using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderNotificationRepository : RepositoryBase<AldebaranDbContext>, IPurchaseOrderNotificationRepository
    {
        public PurchaseOrderNotificationRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderNotifications.AsNoTracking()
                                        .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                                        .Include(i => i.CustomerOrder.Customer)
                                        .Where(w => w.ModifiedPurchaseOrder.PurchaseOrderId == purchaseOrderId)
                                        .ToListAsync(ct);
            }, ct);
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default)
        {
            return await ExecuteQueryAsync(async dbContext =>
            {
                return await dbContext.PurchaseOrderNotifications.AsNoTracking()
                                        .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                                        .Include(i => i.CustomerOrder.Customer)
                                        .Where(w => w.ModifiedPurchaseOrder.ModifiedPurchaseOrderId == modifiedPurchaseOrderId)
                                        .ToListAsync(ct);
            }, ct);
        }

        public async Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                try
                {
                    await dbContext.PurchaseOrderNotifications.AddAsync(purchaseOrderNotification, ct);
                    await dbContext.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    dbContext.Entry(purchaseOrderNotification).State = EntityState.Unchanged;
                    throw;
                }
                return Task.CompletedTask;
            }, ct);
        }

        public async Task UpdateAsync(int purchaseOrderNotificationId, string uid, NotificationStatus status, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrderNotifications.FirstOrDefaultAsync(w => w.PurchaseOrderNotificationId == purchaseOrderNotificationId) ?? throw new KeyNotFoundException($"Notificación de la orden de compra con id {purchaseOrderNotificationId} no existe.");
                try
                {
                    entity.NotificationState = status;
                    entity.NotificationId = uid;
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

        public async Task UpdateNotificationResponseAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            await ExecuteCommandAsync(async dbContext =>
            {
                var entity = await dbContext.PurchaseOrderNotifications.FirstOrDefaultAsync(w => w.NotificationId == notificationId) ?? throw new KeyNotFoundException($"Notificación con id {notificationId} no existe.");

                try
                {
                    entity.NotificationState = status;
                    entity.NotificationSendingErrorMessage = errorMessage;
                    entity.NotificationDate = DateTime.Now;
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
