using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderDetailRepository
    {
        Task<IEnumerable<CustomerOrderDetail>> GetAsync(int customerOrderId, CancellationToken ct = default);
    }

}
