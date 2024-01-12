using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderDetailRepository
    {
        Task<PurchaseOrderDetail?> FindAsync(int purchaseOrderDetailId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderDetail purchaseOrder, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderDetailId, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderDetailId, PurchaseOrderDetail purchaseOrder, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int referenceId, int statusOrder, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
