using Aldebaran.Application.Services.Models;
namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task DeleteAsync(int purchaseOrderId, CancellationToken ct = default);
    }
}
