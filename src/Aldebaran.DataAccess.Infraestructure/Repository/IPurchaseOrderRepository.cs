using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
