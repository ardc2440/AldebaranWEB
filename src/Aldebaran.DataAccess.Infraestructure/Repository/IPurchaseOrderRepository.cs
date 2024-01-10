using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<PurchaseOrder> AddAsync(PurchaseOrder item, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
