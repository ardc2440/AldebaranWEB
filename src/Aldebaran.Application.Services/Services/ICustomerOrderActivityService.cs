using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderActivityService
    {
        Task DeleteAsync(int customerOrderActivityId, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderActivity>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default);
        Task AddAsync(CustomerOrderActivity customerOrderActivity, CancellationToken ct = default);
        Task<CustomerOrderActivity?> FindAsync(int customerOrderActivityId, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderActivityId, CustomerOrderActivity customerOrderActivity, CancellationToken ct = default);
    }
}
