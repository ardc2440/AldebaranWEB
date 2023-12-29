using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerContactRepository
    {
        Task AddAsync(CustomerContact customer, CancellationToken ct = default);
        Task UpdateAsync(int customerId, CustomerContact customer, CancellationToken ct = default);
        Task DeleteAsync(int customerId, CancellationToken ct = default);
        Task<IEnumerable<CustomerContact>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
        Task<CustomerContact?> FindAsync(int customerContactId, CancellationToken ct = default);
    }

}
