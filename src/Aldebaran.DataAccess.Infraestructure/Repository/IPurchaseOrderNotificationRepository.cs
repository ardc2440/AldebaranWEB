using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Enums;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderNotificationRepository
    {
        Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct=default);
        Task<IEnumerable<PurchaseOrderNotification>> GetByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default);
        Task UpdateNotificationStatusAsync(int purchaseOrderNotificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default);
        Task UpdateNotificationStatusAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default);
    }
}
