
using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderInProcessDetailRepository
    {
        Task<IEnumerable<CustomerOrderInProcessDetail>> GetAsync(int customerOrderInProcessId, CancellationToken ct);
    }

}
