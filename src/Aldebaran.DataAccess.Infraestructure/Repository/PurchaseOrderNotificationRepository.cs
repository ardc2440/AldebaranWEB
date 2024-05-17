using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public class PurchaseOrderNotificationRepository : IPurchaseOrderNotificationRepository
    {
        private readonly AldebaranDbContext _context;
        public PurchaseOrderNotificationRepository(AldebaranDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderNotifications.AsNoTracking()
                            .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                            .Include(i => i.CustomerOrder.Customer)
                            .Where(w => w.ModifiedPurchaseOrder.PurchaseOrderId == purchaseOrderId)
                            .ToListAsync(ct);
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrderNotifications.AsNoTracking()
                            .Include(i => i.ModifiedPurchaseOrder.ModificationReason)
                            .Include(i => i.CustomerOrder.Customer)
                            .Where(w => w.ModifiedPurchaseOrder.ModifiedPurchaseOrderId == modifiedPurchaseOrderId)
                            .ToListAsync(ct);
        }

        public async Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default)
        {
            try
            {
                await _context.PurchaseOrderNotifications.AddAsync(purchaseOrderNotification, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(purchaseOrderNotification).State = EntityState.Unchanged;
                throw;
            }
        }
        public async Task UpdateAsync(int purchaseOrderNotificationId, string uid, NotificationStatus status, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrderNotifications.FirstOrDefaultAsync(w => w.PurchaseOrderNotificationId == purchaseOrderNotificationId) ?? throw new KeyNotFoundException($"Notificación de la orden de compra con id {purchaseOrderNotificationId} no existe.");

            try
            {
                entity.NotificationState = status;
                entity.NotificationId = uid;
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                _context.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }
        public async Task UpdateNotificationResponseAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            var entity = await _context.PurchaseOrderNotifications.FirstOrDefaultAsync(w => w.NotificationId == notificationId) ?? throw new KeyNotFoundException($"Notificación con id {notificationId} no existe.");

            try
            {
                entity.NotificationState = status;
                entity.NotificationSendingErrorMessage = errorMessage;
                entity.NotificationDate = DateTime.Now;
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
