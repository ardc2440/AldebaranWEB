using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderActivityService
    {
        Task<PurchaseOrderActivity?> FindAsync(int purchaseOrderActivityId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderActivityId, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderActivityId, PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
