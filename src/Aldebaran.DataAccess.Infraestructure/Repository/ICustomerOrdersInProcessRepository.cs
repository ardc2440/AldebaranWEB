using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrdersInProcessRepository
    {
        Task AddAsync(CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct);
        Task<IEnumerable<CustomerOrdersInProcess>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct);
        Task UpdateAsync(int customerOrderInProcessId, CustomerOrdersInProcess customerOrdersInProcess, CancellationToken ct);
        Task<CustomerOrdersInProcess?> FindAsync(int customerOrderInProcessId, CancellationToken ct = default);
    }

}
