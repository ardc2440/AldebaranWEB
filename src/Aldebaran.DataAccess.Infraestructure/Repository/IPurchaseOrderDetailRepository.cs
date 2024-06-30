using Aldebaran.DataAccess.Entities;
using static Aldebaran.DataAccess.Infraestructure.Repository.PurchaseOrderDetailRepository;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderDetailRepository
    {
        Task<PurchaseOrderDetail?> FindAsync(int purchaseOrderDetailId, CancellationToken ct = default);
        Task AddAsync(PurchaseOrderDetail purchaseOrder, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderDetailId, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderDetailId, PurchaseOrderDetail purchaseOrder, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDetail>> GetByReferenceIdAndStatusOrderAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDetail>> GetByPurchaseOrderIdAsync(int purchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderVariation>> IsValidPurchaseOrderVariation(int providerId, int referenceId, int quantity, int VariationMontNumber, int? purchaseOrderId = null, CancellationToken ct = default);
    }
}
