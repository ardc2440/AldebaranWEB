using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<PurchaseOrder> AddAsync(PurchaseOrder item, CancellationToken ct = default);
        Task CancelAsync(int purchaseOrderId, Reason reason, CancellationToken ct = default);
        Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default);
    }
}
