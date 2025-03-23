using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrdersInProcessRepository
    {
        Task<CustomerOrdersInProcess> AddAsync(CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct);
        Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct);
        Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrdersInProcess, Reason reason, CancellationToken ct);
        Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderInProcessId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task<bool> ExistsAutomaticCustomerOrderInProcess(int customerOrderId, int processSatelliteId, CancellationToken ct);
    }

}
