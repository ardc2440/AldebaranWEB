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
        Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderVariation>> IsValidPurchaseOrderVariation(int providerId, int referenceId, int quantity, int variationMontNumber, int? purchaseOrderId = null, CancellationToken ct = default);
        Task<bool> ExistsDetailByReferenceId(int referenceId, CancellationToken ct = default);
    }
}
