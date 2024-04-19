using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderDetailService
    {
        Task<PurchaseOrderDetail?> FindAsync(int purchaseOrderDetailId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderDetail purchaseOrder, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderDetailId, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderDetailId, PurchaseOrderDetail purchaseOrder, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default);
        List<PurchaseOrderDetail> GetTransitDetailOrders(int statusOrder, int? referenceId = null);
        Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
