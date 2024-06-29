using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderNotificationService
    {
        Task<PurchaseOrderNotification?> FindAsync(int purchaseOrderNotificationId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderNotification>> GetByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderNotificationId, string uid, NotificationStatus status, CancellationToken ct = default);
        Task UpdateNotificationResponseAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default);
        Task NotifyToCustomers(int modifiedPurchaseOrderId, string baseUri, CancellationToken ct = default);
    }
}
