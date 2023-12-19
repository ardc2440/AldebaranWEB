using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderDetailService
    {
        Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int referenceId, int statusOrder, CancellationToken ct = default);
    }
}
