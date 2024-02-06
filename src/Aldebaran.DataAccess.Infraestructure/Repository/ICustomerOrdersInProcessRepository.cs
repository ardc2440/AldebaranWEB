using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrdersInProcessRepository
    {
        Task<CustomerOrdersInProcess> AddAsync(CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct);
        Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct);
        Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct);
        Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderInProcessId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
    }

}
