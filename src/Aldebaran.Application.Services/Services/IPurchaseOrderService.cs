using Aldebaran.Application.Services.Models;
namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task CancelAsync(int purchaseOrderId, Reason reason, CancellationToken ct = default);
        Task<PurchaseOrder> AddAsync(PurchaseOrder purchaseOrder, CancellationToken ct = default);
        Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default);
        Task UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default);
    }
}
