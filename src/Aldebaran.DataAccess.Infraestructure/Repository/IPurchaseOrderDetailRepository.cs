using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IPurchaseOrderDetailRepository
    {
        Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int referenceId, int statusOrder, CancellationToken ct = default);
    }
}
