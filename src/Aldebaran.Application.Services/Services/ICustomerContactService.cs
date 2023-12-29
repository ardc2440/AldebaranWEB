using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerContactService
    {
        Task AddAsync(CustomerContact customerContact, CancellationToken ct = default);
        Task UpdateAsync(int customerContactId, CustomerContact customerContact, CancellationToken ct = default);
        Task DeleteAsync(int customerContactId, CancellationToken ct = default);
        Task<IEnumerable<CustomerContact>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
        Task<CustomerContact?> FindAsync(int customerContactId, CancellationToken ct = default);

    }

}
