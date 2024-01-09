using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrdersInProcessService
    {
        Task AddAsync(CustomerOrdersInProcess customerOrderInProcess, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrderInProcess, CancellationToken ct = default);
        Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default);

    }

}
