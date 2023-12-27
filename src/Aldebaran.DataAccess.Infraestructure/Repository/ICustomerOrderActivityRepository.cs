using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderActivityRepository
    {
        Task DeleteAsync(int customerOrderActivityId, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderActivity>> GetAsync(int customerOrderId, CancellationToken ct = default);
        Task AddAsync(CustomerOrderActivity customerOrderActivity, CancellationToken ct = default);
        Task<CustomerOrderActivity?> FindAsync(int customerOrderActivityId, CancellationToken ct);
        Task UpdateAsync(int customerOrderActivityId, CustomerOrderActivity customerOrderActivity, CancellationToken ct = default);
    }
}
