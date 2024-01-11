using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderActivityService
    {
        Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
