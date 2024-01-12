using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderActivityRepository
    {
        Task<PurchaseOrderActivity?> FindAsync(int purchaseOrderActivityId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderActivityId, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderActivityId, PurchaseOrderActivity purchaseOrderActivity, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderActivity>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
    }

}
