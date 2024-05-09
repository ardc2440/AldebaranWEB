using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderNotificationService
    {
        Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct=default);
        Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default);
        Task UpdateNotificationStatusAsync(int purchaseOrderNotificationId, bool status, string errorMessage, CancellationToken ct = default);
    }
}
