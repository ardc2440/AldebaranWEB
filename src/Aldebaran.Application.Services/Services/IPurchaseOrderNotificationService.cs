using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderNotificationService
    {
        Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct=default);
    }
}
