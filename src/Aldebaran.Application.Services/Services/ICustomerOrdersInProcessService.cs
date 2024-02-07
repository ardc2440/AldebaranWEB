using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrdersInProcessService
    {
        Task<CustomerOrdersInProcess> AddAsync(CustomerOrdersInProcess customerOrderInProcess, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrderInProcess, Reason reason, CancellationToken ct = default);
        Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderInProcessId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
    }
}
